using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfficientDelivery.DAL.Database;
using EfficientDelivery.DAL.Database.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdsAgregator.Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EfficientDelivery_UserController : ControllerBase
    {
        private EfficientDeliveryDbContext _database { get; set; }

        public EfficientDelivery_UserController(EfficientDeliveryDbContext database)
        {
            _database = database;
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            try
            {
                _database.Logs.Add(new Log() { CreatedAt = DateTime.Now, Message = "test" });
                await _database.SaveChangesAsync(); 

            }
            catch (Exception e)
            {
                return Content($"{e.Message} ||||||||||||||||| {e.InnerException.Message}");
            }

            return Content("ok");
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
            _database.Logs.Add(new Log() { CreatedAt = DateTime.Now, Message = "before login check" });
            await _database.SaveChangesAsync();

            var user = _database
                .Users
                .FirstOrDefault(u => u.UserName.ToUpper() == username.ToUpper());



            if (user == null)
                return StatusCode(400, "Користувача з таким логіном немає");

            _database.Logs.Add(new Log() { CreatedAt = DateTime.Now, Message = "before password check" });
            await _database.SaveChangesAsync();

            if (user.Password.ToUpper() != password.ToUpper())
                return StatusCode(400, "Невірний пароль");


            _database.Logs.Add(new Log() { CreatedAt = DateTime.Now, Message = "before token check" });
            await _database.SaveChangesAsync();

            if (user.MobileAppToken?.ToUpper() != mobileToken?.ToUpper())
            {
                _database.Logs.Add(new Log() { CreatedAt = DateTime.Now, Message = "before token update" });
                await _database.SaveChangesAsync();

                user.MobileAppToken = mobileToken;
                _database.Users.Update(user);

                await _database.SaveChangesAsync();
            }

            _database.Logs.Add(new Log() { CreatedAt = DateTime.Now, Message = "returning success responce" });
            await _database.SaveChangesAsync();

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