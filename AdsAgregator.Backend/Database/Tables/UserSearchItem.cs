using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdsAgregator.Backend.Database.Tables
{
    public class UserSearchItem
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Url { get; set; }
    }
}
