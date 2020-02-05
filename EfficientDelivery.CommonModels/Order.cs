using System;

namespace EfficientDelivery.CommonModels
{
    public class Order
    {
        public int OrderId { get; set; }
        public OrderSource OrderSource { get; set; }
        public string DistanceInfo { get; set; }
        public DateTime PublishDate { get; set; }
        public string CargoShippingDateInfo { get; set; }
        public string TransportType { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public string CargoInfo { get; set; }
        public string PaymentInfo { get; set; }
        public string OrderLink { get; set; }
    }
}
