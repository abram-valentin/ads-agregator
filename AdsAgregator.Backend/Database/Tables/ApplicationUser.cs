using Microsoft.AspNetCore.Identity;

namespace AdsAgregator.Backend.Database.Tables
{
    public class ApplicationUser
    {

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MobileAppToken { get; set; }
    }
}
