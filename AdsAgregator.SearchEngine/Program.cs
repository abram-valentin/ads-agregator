using AdsAgregator.CommonModels.Models;
using AdsAgregator.Core.SearchClients;
using AdsAgregator.SearchEngine.Database;
using AdsAgregator.SearchEngine.Database.Tables;
using AdsAgregator.SearchEngine.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace AdsAgregator.SearchEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
            Console.WriteLine("GO");
            SearchEngine.Start();
            Console.ReadLine();
        }

    }

    static class SearchEngine
    {
        private static SearchEngineStatus _status = SearchEngineStatus.Off;
        private static Timer _timer;
        private const int INTERVAL = 20000;
        public static List<Search> Searches { get; set; } = new List<Search>();


        private static async Task UpdateSearchList()
        {
            var dbContext = new AppDbContext();

            var searchItemsFromDb = await dbContext
                .SearchItems
                .Where(s => s.IsActive == true)
                .ToListAsync();


            var newItems = searchItemsFromDb
                .Where(sdb => Searches.Select(s => s.Searchitem.Id).Contains(sdb.Id) == false);


            foreach (var item in newItems)
            {
                var owner = await dbContext.Users.FindAsync(item.OwnerId);

                Searches.Add(new Search(item, owner));
            }

            var itemsToRemove = Searches
                .Where(s => searchItemsFromDb.Select(ni => ni.Id).Contains(s.Searchitem.Id) == false);

            foreach (var item in itemsToRemove)
            {
                Searches.Remove(item);
            }

            foreach (var item in searchItemsFromDb)
            {
                var itemToUpdate = Searches.FirstOrDefault(s => s.Searchitem.Id == item.Id);
                itemToUpdate.Update(item);
            }

        }

        public static async void Start()
        {
            _status = SearchEngineStatus.On;

            await UpdateSearchList();
            await MakeSearch();

            _timer = new Timer(INTERVAL);
            _timer.Elapsed += OnTimerClick;
            _timer.Start();

        }

        public static void Stop()
        {
            _timer?.Stop();
            _status = SearchEngineStatus.Off;
        }

        public static SearchEngineStatus GetEngineStatus()
        {
            return _status;
        }

        private static async void OnTimerClick(object sender, ElapsedEventArgs e)
        {
            await UpdateSearchList();
            try
            {
                await MakeSearch();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task MakeSearch()
        {
            var taskList = new List<Task>();

            foreach (var item in Searches)
            {
                taskList.Add(Task.Run(async () => await item.ProcessSearch()));
            }

            await Task.WhenAll(taskList);
        }
    }

    class Search
    {
        private const int LIST_SIZE = 50;
        private ISearchClient _searchClient;
        private ApplicationUser _user;

        public Database.Tables.SearchItem Searchitem { get; set; }
        public List<AdModel> AdsCache { get; set; } = new List<AdModel>();

        public Search(Database.Tables.SearchItem item, ApplicationUser owner)
        {
            this.Searchitem = item;
            _searchClient = ResolveSearchClient(Searchitem.Url);
            this._user = owner;
        }

        public void Update(Database.Tables.SearchItem searchItem)
        {
            this.Searchitem = searchItem;
        }


        public async Task ProcessSearch()
        {
            var ads = new List<AdModel>();
            try
            {
                ads = await _searchClient.GetAds();

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            if (ads?.Count == 0)
                return;

            var newAds = ads.Where(ad =>
                AdsCache.FirstOrDefault(item =>
                    item.ProviderAdId == ad.ProviderAdId && item.AdSource == ad.AdSource) == null)
                .ToList();

            bool append = AdsCache.Count == 0;

            if (AdsCache.Count > 0)
            {
                newAds.Reverse();
            }


            foreach (var item in newAds)
            {

                if (AdsCache.Count >= LIST_SIZE)
                {
                    AdsCache.RemoveAt(AdsCache.Count - 1);
                }

                if (append)
                {
                    AdsCache.Add(item);

                }
                else
                {
                    AdsCache.Insert(0, item);
                }


                Console.WriteLine($"=======NEW CAR: {item.AdTitle} {item.PriceInfo} {item.CarInfo} =======");


            }

            if (newAds.Count > 0)
            {
                using (var dbContext = new AppDbContext())
                {
                    foreach (var item in newAds)
                    {
                        dbContext.Ads.Add(new Ad 
                        {
                            OwnerId = _user.Id,
                            ProviderAdId = item.ProviderAdId,
                            AdTitle = item.AdTitle,
                            CarInfo = item.CarInfo,
                            ImageLink = item.ImageLink,
                            PriceInfo = item.PriceInfo,
                            AdSource = item.AdSource,
                            AddressInfo = item.AddressInfo,
                            Email = item.Email,
                            CreatedAtInfo = item.CreatedAtInfo,
                            Phone = item.Phone,
                            AdLink = item.AdLink,
                        });
                    }

                    await dbContext.SaveChangesAsync();
                }

                await MessagingService.SendPushNotificationWithData($"({newAds.Count()}) нових авто. {Searchitem.Title}", Searchitem.Description, new Random().Next(1, 9999999), _user.MobileAppToken);
            }




        }

        public ISearchClient ResolveSearchClient(string url)
        {

            if (url.Contains("m.ebay"))
            {
                return new EbayDeMobileSearchClient() { _searchUrl = url };
            }
            else if (url.Contains("ebay-kleinanzeigen.de"))
            {
                return new EbayDeSearchClient() { _searchUrl = url };
            }

            return null;
        }
    }

    public enum SearchEngineStatus
    {
        On = 1,
        Off = 2
    }
}
