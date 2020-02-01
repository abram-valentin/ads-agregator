using CommonEnums = AdsAgregator.CommonModels.Enums;
using CommonModels = AdsAgregator.CommonModels.Models;
using AdsAgregator.DAL.Database;
using AdsAgregator.DAL.Database.Tables;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Threading;

namespace SearchEngine.MobileDe
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MobileDeSearchEngine();

            while (true)
            {
                client
                    .ProcessSearch()
                    .GetAwaiter()
                    .GetResult();
            }
        }
    }



    public class MobileDeSearchEngine
    {
        private AppDbContext dbContext;
        private IWebDriver browser;
        private string apiUrl = "https://adsagregatorbackend.azurewebsites.net/api/";

        public MobileDeSearchEngine()
        {
            dbContext = new AppDbContext();
            browser = new ChromeDriver();
            
        }

        private async Task<List<SearchItem>> GetActiveSearches()
        {
            return await dbContext.SearchItems
                .Where(si => si.AdSource == CommonEnums.AdSource.MobileDe && si.IsActive == true)
                .ToListAsync();
        }

        public async Task ProcessSearch()
        {
            var searchItems = await GetActiveSearches();

            foreach (var item in searchItems)
            {
                var tasks = new List<Task>();

                browser.Navigate().GoToUrl(item.Url);

                tasks.Add(ActLikeHuman(browser));
                
                var content = browser.PageSource;
                var resultList = await MobileDeParser.GetDataFromHtml(content);

                var list = new List<Ad>();
                
                foreach (var resultItem in resultList)
                {
                    list.Add(new Ad
                    { 
                        OwnerId = item.OwnerId,
                        AddressInfo = resultItem.AddressInfo,
                        AdLink = resultItem.AdLink,
                        AdSource = resultItem.AdSource,
                        AdTitle = resultItem.AdTitle,
                        CarInfo = resultItem.CarInfo,
                        CreatedAtInfo = resultItem.CreatedAtInfo,
                        Email = resultItem.Email,
                        ImageLink = resultItem.ImageLink,
                        Phone = resultItem.Phone,
                        PriceInfo = resultItem.PriceInfo,
                        ProviderAdId = resultItem.ProviderAdId
                    });
                }

                tasks.Add(PostAds(item.OwnerId.ToString(), list));

                await Task.WhenAll(tasks);
            }

            for (int i = 0; i < searchItems.Count; i++)
            {
                browser.Navigate().Back();
            }
        }

        private async Task<HttpStatusCode> PostAds(string userId, List<Ad> ads)
        {
            var httpClient = new HttpClient();
            var parameters = new Dictionary<string, string>()
                {
                    { "userId", userId },
                    { "ads", JsonConvert.SerializeObject(ads)},
                };

            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = await httpClient.PostAsync($"{apiUrl}/ads/postads", encodedContent);

            

            return  response.StatusCode;
        }

        private Task ActLikeHuman(IWebDriver driver)
        {
            var random = new Random();

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            js.ExecuteScript("var button = document.getElementById('gdpr-consent-accept-button'); if(button != null){button.click();}");

            var steps = random.Next(3, 10);

            for (int i = 0; i < steps; i++)
            {
                js.ExecuteScript($"window.scroll(0, {random.Next(0, i * 1000)})");

                Thread.Sleep(random.Next(1000, 5000));
            }

            js.ExecuteScript("document.getElementsByClassName('cBox--resultList')[0].click()");

            return Task.CompletedTask;

        }
    }

    public static class MobileDeParser
    {
        public static async Task<List<CommonModels.AdModel>> GetDataFromHtml(string rawHtml)
        {
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
            }

            var list = new List<CommonModels.AdModel>();


            foreach (var item in nodes)
            {
                list.Add(ParseAdItem(item));
            }

            return list;
        }

        private static CommonModels.AdModel ParseAdItem(HtmlNode node)
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


            var model = new CommonModels.AdModel()
            {
                ProviderAdId = HttpUtility.HtmlDecode(adId),
                AdTitle = HttpUtility.HtmlDecode(adTitle),
                CarInfo = HttpUtility.HtmlDecode(carInfo),
                ImageLink = HttpUtility.HtmlDecode(imageLink),
                PriceInfo = HttpUtility.HtmlDecode(priceInfo),
                AdSource = CommonEnums.AdSource.MobileDe,
                AddressInfo = HttpUtility.HtmlDecode(locationInfo),
                CreatedAtInfo = HttpUtility.HtmlDecode(adDateCreated),
                AdLink = adLink
            };


            return model;
        }
    }
}
