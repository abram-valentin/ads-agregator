using EfficientDelivery.DAL.Database;
using EfficientDelivery.DAL.Database.Tables;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EfficientDelivery.SearchEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            Begin:

            var se = new SearchEngine();

            try
            {
                se.Start().GetAwaiter().GetResult();
                Console.ReadLine();
                Console.ReadLine();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Thread.Sleep(10000);
                goto Begin;
            }
        }
    }

    public class SearchEngine
    {
        private string apiUrl = "https://adsagregatorbackend.azurewebsites.net/api/";


        public async Task Start()
        {
            EfficientDeliveryDbContext dbContext = new EfficientDeliveryDbContext();

            while (true)
            {
                DateTime startTime = DateTime.Now;

                var activeSearches = await dbContext
                    .Searches
                    .Where(s => s.IsActive == true)
                    .ToListAsync();

                var tasks = new List<Task>();

                foreach (var item in activeSearches)
                {
                    tasks.Add(Search(item));
                }

                await Task.WhenAll(tasks);

                if (DateTime.Now.Subtract(startTime).Seconds < 5)
                {
                    Thread.Sleep(3000);
                }

                Console.WriteLine($"++++++++++ Last request {DateTime.Now.ToLongTimeString()} ++++++++");
            }

        }


        private async Task Search(SearchItem searchItem)
        {
            ISearchClient searchClient = ResolveSearchClient(searchItem.Url);

            var orders = await searchClient.GetOrders(searchItem.Url);

            var result = await PostAds(searchItem.OwnerId.ToString(), orders);

            if (result != HttpStatusCode.OK)
            {
                throw new Exception("Orders are not posted to server");
            }
        }

        private async Task<HttpStatusCode> PostAds(string userId, List<EfficientDelivery.CommonModels.Order> orders)
        {
            var httpClient = new HttpClient();
            var parameters = new Dictionary<string, string>()
                {
                    { "userId", userId },
                    { "ordersJson", JsonConvert.SerializeObject(orders)},
                };

            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = await httpClient.PostAsync($"{apiUrl}/EfficientDelivery_Orders/PostOrders", encodedContent);

            var content = await response.Content.ReadAsStringAsync();
            

            return response.StatusCode;
        }

        private ISearchClient ResolveSearchClient(string url)
        {
            if (url.ToLower().Contains("lardi-trans"))
                return new LardiTransSearchClient();
            else if (url.ToLower().Contains("della.ua"))
                return new DellaSearchClient();
            else
                throw new Exception("No other search clients are supported yet");
        }
    }
}
