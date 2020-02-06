﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EfficientDelivery.DAL.Database.Tables
{
    public class Order: EfficientDelivery.CommonModels.Order
    {
        [Key]
        public override int Id { get; set; }
        [ForeignKey("Owner")]
        public int OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
    }
}
