using System;

namespace EfficientDelivery.DAL.Database.Tables
{
    public class Log
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
