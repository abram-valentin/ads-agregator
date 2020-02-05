using EfficientDelivery.DAL.Database.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AdsAgregator.DAL.Database
{
    public class AppDbContext : DbContext
    {
        public IConfigurationRoot Configuration { get; set; }

        public AppDbContext()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Order> Orders { get; set; }
    }
}
