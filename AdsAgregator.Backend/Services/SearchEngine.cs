using AdsAgregator.Backend.Database;
using AdsAgregator.Backend.Database.Tables;
using AdsAgregator.Core.SearchClients;
using AdsAgregator.Core.SearchModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace AdsAgregator.Backend.Services
{
    public class SearchEngine
    {
        private Timer _timer;
        private const int INTERVAL = 10000;
        public List<Search> Searches { get; set; } = new List<Search>();


        private async Task UpdateSearchList()
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

            foreach (var item in Searches)
            {
                var searchItemToDelete = newItems.FirstOrDefault(i => i.Id == item.Searchitem.Id);

                if (searchItemToDelete != null)
                {
                    Searches.Remove(item);
                }
            }

                    
        }

        public async void Start()
        {

            await UpdateSearchList();
            await MakeSearch();

            _timer = new Timer(INTERVAL);
            _timer.Elapsed += OnTimerClick;
            _timer.Start();
        }


        public void Stop()
        {
            _timer.Stop();
        }

        private async void OnTimerClick(object sender, ElapsedEventArgs e)
        {
            await UpdateSearchList();
            await MakeSearch();
        }

        private async Task MakeSearch()
        {
            var taskList = new List<Task>();

            foreach (var item in Searches)
            {
                taskList.Add(Task.Run(async () => await item.ProcessSearch()));
            }

            await Task.WhenAll(taskList);
        }
    }

    public class Search
    {
        private const int LIST_SIZE = 50;
        private ISearchClient _searchClient;
        private ApplicationUser _user;

        public SearchItem Searchitem { get; set; }
        public List<AdModel> AdsCache { get; set; } = new List<AdModel>();

        public Search(SearchItem item, ApplicationUser owner)
        {
            this.Searchitem = item;
            _searchClient = ResolveSearchClient(Searchitem.Url);
            this._user = owner;
        }


        public async Task ProcessSearch()
        {
            var ads = await _searchClient.GetAds();

            if (ads?.Count == 0)
                return;

            var newAds = ads.Where(ad =>
                AdsCache.FirstOrDefault(item =>
                    item.Id == ad.Id && item.AdSource == ad.AdSource) == null)
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


                Debug.WriteLine($"=======NEW CAR: {item.AdTitle} {item.PriceInfo} {item.CarInfo} =======");
                await MessagingService.SendPushNotificationWithData("new car", $"{item.AdTitle} {item.PriceInfo}","", _user.MobileAppToken);
            }
        }

        public  ISearchClient ResolveSearchClient(string url)
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
}
