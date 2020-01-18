using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdsAgregator.Backend.Database;
using AdsAgregator.Backend.Database.Tables;
using AdsAgregator.Backend.Services;
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
        private DbLogger _dbLogger { get; set; }

        public UserController(AppDbContext database, DbLogger dbLogger)
        {
            _database = database;
            _dbLogger = dbLogger;
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            return Ok();
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
            _dbLogger.Log("getting user from database", DateTime.Now);

            var user = _database
                .Users
                .FirstOrDefault(u => u.UserName.ToUpper() == username.ToUpper());

            _dbLogger.Log("checking if user is not null", DateTime.Now);

            if (user == null)
                return StatusCode(400, "Користувача з таким логіном немає");

            _dbLogger.Log("checking password", DateTime.Now);

            if (user.Password.ToUpper() != password.ToUpper())
                return StatusCode(400, "Невірний пароль");


            _dbLogger.Log("checking token", DateTime.Now);

            if (user.MobileAppToken?.ToUpper() != mobileToken?.ToUpper())
            {
                _dbLogger.Log("updating token", DateTime.Now);

                user.MobileAppToken = mobileToken;
                _database.Users.Update(user);

                _dbLogger.Log("Saving to database", DateTime.Now);
                await _database.SaveChangesAsync();
            }

            return StatusCode(200, user);
        }

        [HttpGet]
        public async Task<IActionResult> SetMobileToken(int userId, string mobileToken)
        {
            var user = _database
                .Users
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return StatusCode(400, "Користувача з таким Id немає");

            if (user.MobileAppToken?.ToUpper() != mobileToken?.ToUpper())
            {
                user.MobileAppToken = mobileToken ?? "";
                _database.Users.Update(user);
                await _database.SaveChangesAsync();
            }


            return StatusCode(200, user);
        }
    }
}