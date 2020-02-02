using AdsAgregator.DAL.Database;
using AdsAgregator.DAL.Database.Tables;
using System;

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
