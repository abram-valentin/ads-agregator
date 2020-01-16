using AdsAgregator.Backend.Database;
using AdsAgregator.Backend.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdsAgregator.Backend.Services
{
    public class DbLogger
    {
        public AppDbContext _dbContext { get; set; }

        public DbLogger(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public void Log(string message, DateTime createdAt)
        {
            _dbContext.Logs.Add(new Log() {Message = message, CreatedAt = createdAt });
            _dbContext.SaveChanges();
        }
    }
}
