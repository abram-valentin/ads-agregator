using EfficientDelivery.CommonModels;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace EfficientDelivery.SearchEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new LardiTransSearchClient();
            client.GetOrders("https://lardi-trans.com/gruz/c640h640b64i20i46.html")
                .GetAwaiter()
                .GetResult();
        }
    }

    class LardiTransSearchClient
    {
        public async Task<int> GetOrders(string url)
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
                return 0;
            }

            GetDataFromHtml(html);

            return 1;
        }

        private List<Order> GetDataFromHtml(string html)
        { 
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            IEnumerable<HtmlNode> nodes =
               document.DocumentNode.Descendants(0)
               .Where(n => n.HasClass("ps_search-result_data-item"));

            if (nodes == null | nodes.Count() == 0)
                throw new Exception("No orders was extracted from html");

            var orders = new List<Order>();

            foreach (var item in nodes)
            {
                orders.Add(ParseOrderItem(item));
            }

            return orders;
        }


        private Order ParseOrderItem(HtmlNode node)
        {
            var orderId = node.Descendants(0)
               .FirstOrDefault(n => n.ChildAttributes("data-ps-id").Count() > 0)
               ?.GetAttributeValue("data-ps-id", null);

            var distanceInfo = node.Descendants(0)
                .FirstOrDefault(n => n.ChildAttributes("data-distance").Count() > 0)
                ?.GetAttributeValue("data-distance", null);

            var publishDateScript = node.Descendants(0)
                .FirstOrDefault(n =>
                    n.Name == "script" && n.Attributes
                        .Select(a => a.Name == "data-name" && a.Value == "localTime")?.Count() > 0)
                .InnerHtml;
            
            double dateMiliseconds = 0;
            
            var dateMilisecondsStr = Regex.Match(publishDateScript, @"\d+").Value;

            double.TryParse(dateMilisecondsStr, out dateMiliseconds);
            
            var publishDate = new DateTime()
                .AddMilliseconds(dateMiliseconds);

            
            var cargoShippingDateInfo = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("ps_data_load-date"))
                ?.InnerHtml;

            if (!string.IsNullOrWhiteSpace(cargoShippingDateInfo))
                cargoShippingDateInfo = HttpUtility.HtmlDecode(cargoShippingDateInfo);

            var transportType = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("ps_data_transport"))
                ?.InnerHtml;


            var locationFrom = node.Descendants(0)
               .FirstOrDefault(n => n.HasClass("ps_search-result_data-from"))
               ?.InnerText.RemoveEscapes();

            locationFrom = HttpUtility.HtmlDecode(locationFrom);


            var locationTo = node.Descendants(0)
               .FirstOrDefault(n => n.HasClass("ps_search-result_data-where"))
               ?.InnerText.RemoveEscapes();
            
            locationTo = HttpUtility.HtmlDecode(locationTo);

            var cargoInfo = node.Descendants(0)
               .FirstOrDefault(n => n.HasClass("ps_data-cargo"))
               ?.InnerText.RemoveEscapes();


            var paymentInfo = node.Descendants(0)
               .FirstOrDefault(n => n.HasClass("ps_data-payment"))
               ?.InnerText.RemoveEscapes();

            paymentInfo = HttpUtility.HtmlDecode(paymentInfo);


            var orderLink = "https://lardi-trans.com/" + node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("ps_options_list__item"))
                .GetAttributeValue("href", "");


            


            return new Order 
            {
                OrderId = int.Parse(orderId),
                OrderSource = OrderSource.lardi_trans,
                DistanceInfo = distanceInfo,
                PublishDate = publishDate,
                CargoShippingDateInfo = cargoShippingDateInfo,
                TransportType = transportType,
                LocationFrom = locationFrom,
                LocationTo = locationTo,
                CargoInfo = cargoInfo,
                PaymentInfo = paymentInfo,
                OrderLink = orderLink
            };
        }

        private async Task<string> GetRawData(string url)
        {
            HttpClient httpClient = new HttpClient( );
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36");

            var responce = await httpClient.GetAsync(url);

            if (responce.IsSuccessStatusCode)
                return await responce.Content.ReadAsStringAsync();
            else
                throw new Exception("Request does not indicated success");
        }
    }
}
