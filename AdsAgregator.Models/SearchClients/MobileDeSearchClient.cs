using AdsAgregator.CommonModels.Enums;
using AdsAgregator.CommonModels.Models;
using AdsAgregator.Core.Utilities;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdsAgregator.Core.SearchClients
{
    public class MobileDeSearchClient : ISearchClient
    {
        public string _searchUrl;
        private IWebDriver browser;


        public MobileDeSearchClient()
        {
            browser = new ChromeDriver();
        }

        public async Task<List<AdModel>> GetAds()
        {
            var data = await RequestHtmlData(_searchUrl);
            var items = await GetDataFromHtml(data);

            return items;
        }


        private async Task<string> RequestHtmlData(string url)
        {
            browser.Navigate().GoToUrl(url);
            var content = browser.PageSource;

          

            await ActLikeHuman();

            

            return content;
        }

        private async Task<string> ReopenBrowser()
        {
            this.browser.Close();
            this.browser = new ChromeDriver();
            return await RequestHtmlData(_searchUrl);
        }


        private async Task ActLikeHuman()
        {
            var random = new Random();

            IJavaScriptExecutor js = (IJavaScriptExecutor)browser;

            js.ExecuteScript("var button = document.getElementById('gdpr-consent-accept-button'); if(button != null){button.click();}");

            var steps = random.Next(3, 5);

            for (int i = 0; i < steps; i++)
            {
                js.ExecuteScript($"window.scroll(0, {random.Next(0, i*1000)})");

                await Task.Delay(random.Next(1000, 3000));
            }

            js.ExecuteScript("document.getElementsByClassName('cBox--resultList')[0].click()");

        }

        public async Task<List<AdModel>> GetDataFromHtml(string rawHtml)
        {
            Begin:
            var document = new HtmlDocument();
            document.LoadHtml(rawHtml);

            IEnumerable<HtmlNode> nodes =
                document.DocumentNode.Descendants(0)
                .Where(n => n.HasClass("cBox-body--resultitem") || n.HasClass("cBox-body--eyeCatcher"));


            if (nodes is null | nodes.Count() == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("====================== UPDATE COOKIE ============================");
                Console.ResetColor();

                rawHtml = await ReopenBrowser();
                goto Begin;
            }

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
                .FirstOrDefault(n => n.ChildAttributes("data-ad-id").Count() > 0)
                ?.GetAttributeValue("data-ad-id", null);

            var imageLink = string.Empty;

            var adHref = node.Descendants(0)
                .FirstOrDefault(n => n.ChildAttributes("href").Count() > 0)
                ?.GetAttributeValue("href", null);

          
            var headlineBlock = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("headline-block"));

            var adTitle = headlineBlock.ChildNodes?.Count == 3 ?
                headlineBlock.ChildNodes[1].InnerText 
                : headlineBlock.ChildNodes[0].InnerText;

            
            
            var adLink = adHref;

            var carInfo = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("rbt-regMilPow")).InnerText;

            var locationInfo = string.Empty; //node.Descendants(0)
            //    .Where(n => n.HasClass("g-col-10")).Last().InnerText;

            var priceInfo = node.Descendants(0)
                .FirstOrDefault(n => n.HasClass("price-block")).InnerText;



            var adDateCreated = string.Empty;


            var model = new AdModel()
            {
                ProviderAdId = HttpUtility.HtmlDecode(adId),
                AdTitle = HttpUtility.HtmlDecode(adTitle),
                CarInfo = HttpUtility.HtmlDecode(carInfo) ,
                ImageLink = HttpUtility.HtmlDecode(imageLink) ,
                PriceInfo = HttpUtility.HtmlDecode(priceInfo) ,
                AdSource = AdSource.MobileDe,
                AddressInfo = HttpUtility.HtmlDecode(locationInfo) ,
                CreatedAtInfo = HttpUtility.HtmlDecode(adDateCreated) ,
                AdLink = adLink
            };


            return model;
        }

        public string GetCookies()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "settings.json");

            using (var sr = new StreamReader(path))
            {
                var settings = sr.ReadToEnd();

                var jObject = JObject.Parse(settings);

                return (string)jObject["MobileDe"]["cookie"];
            }
        }
    }
}
