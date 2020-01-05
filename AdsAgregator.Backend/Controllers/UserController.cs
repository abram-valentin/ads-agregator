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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private AppDbContext _database{ get; set; }

        public UserController(AppDbContext database)
        {
            _database = database;
        }


        [HttpGet]
        public async Task<IActionResult> Register(string username, string password, string mobileToken)
        {
            var user = _database
                .Users
                .FirstOrDefault(u => u.UserName.ToUpper() == username.ToUpper());

            if (user != null)
                return StatusCode(400, "Користувача з таким логіном вже існує");

            var entity = new ApplicationUser()
            {
                UserName = username,
                Password = password,
                MobileAppToken = mobileToken
            };

            _database.Users.Add(entity);

            await _database.SaveChangesAsync();

            return StatusCode(201, entity);
        }


        [HttpGet]
        public async Task<IActionResult> SignIn(string username, string password, string mobileToken)
        {
            var user = _database
                .Users
                .FirstOrDefault(u => u.UserName.ToUpper() == username.ToUpper());

            if (user == null)
                return StatusCode(400, "Користувача з таким логіном немає");

            if (user.Password.ToUpper() != password.ToUpper())
                return StatusCode(400, "Невірний пароль");

            if (user.MobileAppToken.ToUpper() != mobileToken.ToUpper())
            {
                user.MobileAppToken = mobileToken;
                _database.Users.Update(user);
                await _database.SaveChangesAsync();
            }

            return StatusCode(200, user);
        }
    }
}