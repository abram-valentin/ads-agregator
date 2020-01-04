using AdsAgregator.Core.SearchClients;
using AdsAgregator.Core.SearchEnums;
using AdsAgregator.Core.SearchModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;

namespace AdsAgregator.Core
{
    public class SearchEngine
    {

        private List<AdModel> _ads = new List<AdModel>();
        public List<AdModel> Ads
        {
            get 
            { 
                return _ads; 
            }
            set 
            { 
                _ads = value; 
                OnAdsListChanged(null); 
            }
        }

        private Timer _timer;
        private AdSearchRequestModel _searchRequestModel;


        public List<ISearchClient> SearchClients { get; set; } = new List<ISearchClient>();


        public SearchEngine(AdSearchRequestModel requestModel, IEnumerable<AdSource> sources)
        {
            this._searchRequestModel = requestModel;
            InitSearchClients(sources);
        }

        public async Task StartSearch(int interval)
        {
            _timer = new Timer(interval);
            _timer.Elapsed += Search;
            _timer.Start();
        }

        public void StopSearch()
        {
            _timer.Stop();
        }


        private void InitSearchClients(IEnumerable<AdSource> adSources)
        {
            foreach (var item in adSources)
            {
                switch (item)
                {
                    case AdSource.Ebay: SearchClients.Add(new EbayDeSearchClient() { _searchUrl = "https://www.ebay-kleinanzeigen.de/s-autos/c216+autos.ez_i:2006," });
                        break;
                    case AdSource.MobileDe: throw new Exception("This client not implemented yet");
                    default:
                        break;
                }
            }
        }



        private async void Search(object sender, ElapsedEventArgs e)
        {
            foreach (var client in SearchClients)
            {
                var ads = await client.GetAds();


                if (ads?.Count > 0)
                {
                    var newAds = ads.Where(ad => this.Ads.FirstOrDefault(item => item.Id == ad.Id && item.AdSource == ad.AdSource) == null);
                    if (newAds?.Count() > 0)
                    { 
                        Ads.AddRange(newAds);
                        OnAdsListChanged(new AdListChangedEventArgs { AdsAdded = ads });
                    }
                }

            }
        }

        public event EventHandler<AdListChangedEventArgs> AdsListChanged;

        private void OnAdsListChanged(AdListChangedEventArgs eventArgs)
        {
            AdsListChanged?.Invoke(this, eventArgs);
        }
    }


    public class AdListChangedEventArgs : EventArgs
    {
        public List<AdModel> AdsAdded { get; set; }
    }
}
