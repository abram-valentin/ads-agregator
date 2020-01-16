using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdsAgregator.Backend.Database.Tables
{
    public class Log
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
