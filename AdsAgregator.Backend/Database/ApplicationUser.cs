using Microsoft.AspNetCore.Identity;

namespace AdsAgregator.Backend.Database
{
    public class ApplicationUser : IdentityUser
    {
        public string MobileAppToken { get; set; }
    }
}
