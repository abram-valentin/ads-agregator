using System;
using System.Collections.Generic;
using System.Text;

namespace EfficientDelivery.DAL.Database.Tables
{
    public class Order: EfficientDelivery.CommonModels.Order
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
    }
}
