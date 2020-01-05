using AdsAgregator.Core.SearchModels;
using HtmlAgilityPack;
using System;
using Fizzler;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using AdsAgregator.Core.SearchEnums;
using AdsAgregator.Core.Utilities;
using System.IO;

namespace AdsAgregator.Core.SearchClients
{

    public class EbayDeSearchClient : ISearchClient
    {
        public string _searchUrl;

        public EbayDeSearchClient()
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

            var stream = await httpClient.GetStreamAsync(url);

            var sr = new StreamReader(stream);


            var res = await sr.ReadToEndAsync();

            return res;
        }

        private List<AdModel> GetDataFromHtml(string rawHtml)
        {
            var document = new HtmlDocument();
            document.LoadHtml(rawHtml);

            IEnumerable<HtmlNode> nodes =
                document.DocumentNode.Descendants(0)
                .Where(n => n.HasClass("lazyload-item") && !n.HasClass("badge-topad"));


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

            var adId = node.Descendants(0)
                .FirstOrDefault(n => n.ChildAttributes("data-adid").Count() > 0)
                ?.GetAttributeValue("data-adid", null);

            var imageLink = node.Descendants(0)
                .FirstOrDefault(n => n.ChildAttributes("data-imgsrc").Count() > 0)
                ?.GetAttributeValue("data-imgsrc", null);

            var adHref = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("ellipsis"));

            var adTitle = adHref.InnerText;

            var adLink = $"https://www.ebay-kleinanzeigen.de {adHref.GetAttributeValue("href", null)}";

            var carInfo = node.Descendants(0)
                .Where(n => n.HasClass("simpletag"))
                .Select(n => n.InnerText);

            var PriceAndLocation = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("aditem-details"))
                .InnerText
                .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();


            for (int i = 0; i < PriceAndLocation.Count; i++)
            {
                PriceAndLocation[i] = PriceAndLocation[i].Trim();
            }

            var adDateCreated = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("aditem-addon"))
                .InnerText;

            adDateCreated = string.Join(" ", adDateCreated.RemoveEscapes()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            var model = new AdModel()
            {
                Id = adId,
                AdTitle = adTitle,
                CarInfo = string.Join(" ", carInfo),
                ImageLink = imageLink,
                PriceInfo = string.Join(" ", PriceAndLocation.Take(PriceAndLocation.Count - 2)),
                AdSource = AdSource.Ebay,
                AddressInfo = string.Join(" ", PriceAndLocation.Skip(PriceAndLocation.Count - 2).Take(2)),
                CreatedAtInfo = adDateCreated,
                AdLink = adLink
            };


            return model;
        }

    }

}

