using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using AdsAgregator.CommonModels.Models;
using AdsAgregator.CommonModels.Enums;

namespace AdsAgregator.Core.SearchClients
{
    public class EbayDeMobileSearchClient : ISearchClient
    {
        public string _searchUrl;

        public EbayDeMobileSearchClient()
        {

        }

        public async Task<List<AdModel>> GetAds()
        {
            var data = await RequestHtmlData(_searchUrl);
            var items = GetDataFromHtml(data);

            return items;
        }


        private async Task<string> RequestHtmlData(string url)
        {
            HttpClient httpClient = new HttpClient();


            Uri uri = new Uri(url);

            string res = string.Empty;

            try
            {
                var stream = await httpClient.GetStreamAsync(url);

                var sr = new StreamReader(stream);


                res = await sr.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            return res;
        }

        private List<AdModel> GetDataFromHtml(string rawHtml)
        {
            var document = new HtmlDocument();
            document.LoadHtml(rawHtml);

            IEnumerable<HtmlNode> nodes =
                document.DocumentNode.Descendants(0)
                .Where(n => n.HasClass("adlist--item") && !n.HasClass("is-feature-topad"));


            if (nodes is null | nodes.Count() == 0)
                return new List<AdModel>();

            var list = new List<AdModel>();

            foreach (var item in nodes)
            {
                list.Add(ParseAdItem(item));
            }

            return list;
        }

        private AdModel ParseAdItem(HtmlNode node)
        {
            var document = new HtmlDocument();
            document.LoadHtml(node.InnerHtml);

            var adId = node.GetAttributeValue("data-adid", null);

            var imageLink = node.Descendants(0)
                .FirstOrDefault(n => n.ChildAttributes("data-src").Count() > 0)
                ?.GetAttributeValue("data-src", null);

            var adHref = node.Descendants("a").FirstOrDefault();

            var adTitle = adHref.InnerText;

            var adLink = $"https://m.ebay-kleinanzeigen.de{adHref.GetAttributeValue("href", null)}";

            var carInfo = node.Descendants(0)
                .Where(n => n.HasClass("simpletag"))
                .Select(n => n.InnerText);

            var priceInfo = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("adlist--item--price"))
                .InnerText;

            var locationInfo = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("adlist--item--info--location"))
                .InnerText;


            var adDateCreated = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("adlist--item--info--date"))
                .InnerText;

           

            var model = new AdModel()
            {
                ProviderAdId = adId,
                AdTitle = adTitle,
                CarInfo = string.Join(" ", carInfo),
                ImageLink = imageLink,
                PriceInfo = priceInfo,
                AdSource = AdSource.Ebay,
                AddressInfo = locationInfo,
                CreatedAtInfo = adDateCreated,
                AdLink = adLink
            };


            return model;
        }

    }

}

