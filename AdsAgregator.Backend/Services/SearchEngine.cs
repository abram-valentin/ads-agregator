using AdsAgregator.Backend.Database;
using AdsAgregator.Backend.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdsAgregator.Backend.Services
{
    public class SearchEngine
    {
        private AppDbContext _database = new AppDbContext();

    }

    public class UserSearch
    {
        public ApplicationUser User { get; set; }
        public List<UserSearchItem> SearchItems { get; set; }
    }

    
}
