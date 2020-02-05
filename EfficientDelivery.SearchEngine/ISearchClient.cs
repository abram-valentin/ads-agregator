using EfficientDelivery.CommonModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EfficientDelivery.SearchEngine
{
    interface ISearchClient
    {
        Task<List<Order>> GetOrders(string url);
    }
}