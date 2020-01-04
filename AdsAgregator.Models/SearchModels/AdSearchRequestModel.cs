using AdsAgregator.Core.SearchEnums;

namespace AdsAgregator.Core.SearchModels
{
    public class AdSearchRequestModel
    {
        public int LocationCode { get; set; }
        public int FirstRegistrationYearFrom { get; set; }
        public int FirstRegistrationYearTo { get; set; }
        public int PriceFrom { get; set; }
        public int PriceTo { get; set; }
        public SearchResultSortingType SortingType { get; set; }
        public AdProviderType AdProviderType { get; set; }
    }
}
