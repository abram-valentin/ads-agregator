using EfficientDelivery.CommonModels;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EfficientDelivery.SearchEngine
{
    class DellaSearchClient : ISearchClient
    {
        public async Task<List<Order>> GetOrders(string url)
        {
            string html = string.Empty;
            try
            {
                html = await GetRawData(url);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            return GetDataFromHtml(html, url);
        }


        private async Task<string> GetRawData(string url)
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36");
            var responce = await httpClient.GetAsync(url);

            if (responce.IsSuccessStatusCode)
                return await responce.Content.ReadAsStringAsync();
            else
                throw new Exception("Request does not indicated success");
        }


        private List<Order> GetDataFromHtml(string html, string url)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            var nodes = document.DocumentNode.Descendants(0)
                    .Where(n => n.HasClass("request_level_ms"));

            var orders = new List<Order>();

            foreach (var item in nodes)
            {
                if (item.Descendants(0).FirstOrDefault(i => i.HasClass("ADV_POS")) == null)
                {
                    orders.Add(ParseItem(item, url));
                }
            }


            return orders;
        }


        private Order ParseItem(HtmlNode node, string url)
        {
            var orderId = long.Parse(node.GetAttributeValue("request_id", "0"));

            var pointA = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("request_distance"))
                .Descendants(0)
                .FirstOrDefault(n => n.Name == "span");

            var pointB = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("request_distance"))
                .Descendants(0)
                .LastOrDefault(n => n.Name == "span");

            var locationFrom = $"{pointA.InnerText} {pointA.Attributes["title"].Value}";
            var locationTo = $"{pointB.InnerText} {pointB.Attributes["title"].Value}";

            var distanceInfo = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("distance_link"))?.InnerText;

            var cargoShippingDateInfo_raw = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("multi_date"))
                ?.InnerText
                .RemoveEscapes();

            var cargoShippingDateInfo = HttpUtility.HtmlDecode(cargoShippingDateInfo_raw);

            var transportType = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("truck"))?.InnerText;

            var cargoWeight = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("weight"))?.InnerText;

            var cargoCube = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("cube"))?.InnerText;

            var additionalInfo_raw = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("m_txt_gr"))?.InnerText.RemoveEscapes();

            var additionalInfo = HttpUtility.HtmlDecode(additionalInfo_raw);

            var cargoInfo = $"{cargoWeight} {cargoCube} {additionalInfo}";

            var priceInfo_raw = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("m_comment") && n.HasClass("pr_10"))
                ?.InnerText
                .RemoveEscapes();

            var paymentInfo = HttpUtility.HtmlDecode(priceInfo_raw);


            var orderLink = "https://della.ua/" + node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("request_distance"))
                ?.GetAttributeValue("href", "");

            var publishInfo_raw = HttpUtility.HtmlDecode(node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("izm_razm"))
                ?.InnerHtml
                .RemoveEscapes()).Replace("<br>", " ").Split(" ")[1];

            var year = DateTime.Now.Year;
            var day = int.Parse(publishInfo_raw.Substring(0, 2));
            var month = int.Parse(publishInfo_raw.Substring(3, 2));
            var time_hours = int.Parse(publishInfo_raw.Substring(7, 2));
            var time_minutes = int.Parse(publishInfo_raw.Substring(10, 2));

            var publishDate = new DateTime(year, month, day, time_hours, time_minutes, 0);

            return new Order
            {
                OrderId = orderId,
                LocationFrom = locationFrom,
                LocationTo = locationTo,
                DistanceInfo = distanceInfo,
                CargoShippingDateInfo = cargoShippingDateInfo,
                TransportType = transportType,  
                CargoInfo = cargoInfo,
                PaymentInfo = paymentInfo,
                OrderLink = url,
                PublishDate = publishDate,
                OrderSource = OrderSource.della
            };
        }
    }
}
