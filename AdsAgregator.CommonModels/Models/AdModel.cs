using AdsAgregator.CommonModels.Enums;
using System;

namespace AdsAgregator.CommonModels.Models
{
    public class AdModel
    {
        public virtual int Id { get; set; }

        /// <summary>
        /// Id of ad, got from ad provider
        /// </summary>
        public string ProviderAdId { get; set; }

        /// <summary>
        /// Title of ad
        /// </summary>
        public string AdTitle { get; set; }

        /// <summary>
        /// Car info
        /// </summary>
        public string CarInfo { get; set; }

        /// <summary>
        /// Ad image link
        /// </summary>
        public string ImageLink { get; set; }

        /// <summary>
        /// Price info
        /// </summary>
        public string PriceInfo { get; set; }

        /// <summary>
        /// Source of ad. Like Ebay or mobile.de
        /// </summary>
        public AdSource AdSource { get; set; }

        /// <summary>
        /// Address info of ad
        /// </summary>
        public string AddressInfo { get; set; }

        /// <summary>
        /// Email of ad owner
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Date and time ad was created by its owner
        /// </summary>
        public string CreatedAtInfo { get; set; }

        /// <summary>
        /// Phone of ad owner
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Full url of ad page
        /// </summary>
        public string AdLink { get; set; }

    }
}
