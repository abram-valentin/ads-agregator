using EfficientDelivery.DAL.Database.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EfficientDelivery.DAL.Database
{
    public class EfficientDeliveryDbContext : DbContext
    {
        public IConfigurationRoot Configuration { get; set; }

        public EfficientDeliveryDbContext()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=tcp:ads-agregator-dbserver.database.windows.net,1433;Initial Catalog=EfficientDeliveryDb;Persist Security Info=False;User ID=Valentin;Password=Valik123852456.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<SearchItem> Searches { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Log> Logs { get; set; }

    }
}
