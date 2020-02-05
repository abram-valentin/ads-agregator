using AdsAgregator.DAL.Database.Tables;
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
            optionsBuilder.UseSqlServer("Server=tcp:adsagregatorbackenddbserver.database.windows.net,1433;Initial Catalog=AdsAgregatorBackend_db;Persist Security Info=False;User ID=Valentin;Password=Valik123852456.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Ad> Ads { get; set; }
        public DbSet<SearchItem> SearchItems { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}
