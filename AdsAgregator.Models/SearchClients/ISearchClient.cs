using AdsAgregator.Core.SearchModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdsAgregator.Core.SearchClients
{
    public interface ISearchClient
    {
        /// <summary>
        /// Returns new ads from source
        /// </summary>
        /// <returns></returns>
        Task<List<AdModel>> GetAds();
    }

}
