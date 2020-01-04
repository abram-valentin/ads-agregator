using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdsAgregator.Backend.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdsAgregator.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private AppDbContext _database{ get; set; }
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(AppDbContext database, SignInManager<ApplicationUser> signInManager)
        {
            _database = database;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> SignIn(string username, string password, string mobileToken)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, true, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return StatusCode(400);
            }

            var user = _database.Users.FirstOrDefault(u => u.UserName == username);

            bool isTokenValid = user.MobileAppToken.ToUpper() == mobileToken.ToUpper();

            if (!isTokenValid)
            {
                user.MobileAppToken = mobileToken;
                _database.Users.Update(user);
                await _database.SaveChangesAsync();
            }

            return StatusCode(200);
        }
    }
}