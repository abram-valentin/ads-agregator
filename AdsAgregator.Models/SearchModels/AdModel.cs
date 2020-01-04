using AdsAgregator.Core.SearchEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdsAgregator.Core.SearchModels
{
    public class AdModel
    {
        public string Id { get; set; }
        public string AdTitle { get; set; }
        public string CarInfo { get; set; }
        public string ImageLink { get; set; }
        public string PriceInfo { get; set; }
        public AdSource AdSource { get; set; }
        public string AddressInfo { get; set; }
        public string Email { get; set; }
        public string CreatedAtInfo { get; set; }
        public string Phone { get; set; }
        public string AdLink { get; set; }
        
        /// <summary>
        /// Date of creation of object 
        /// </summary>
        public TimeSpan CreatedAt_Internal { get; set; } = DateTime.Now.TimeOfDay;
    }
}
