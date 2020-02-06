using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdsAgregator.Backend.Services;
using AdsAgregator.CommonModels.Models;
using AdsAgregator.DAL.Database;
using AdsAgregator.DAL.Database.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AdsAgregator.Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private AppDbContext _dbContext;
        private const string _firebaseServerApiKey = "AAAAtc0y0OE:APA91bFyopTw_73HY9KatdjbGV_cYLWGtmLakga-yHeyO7kV6XPXLinpPGZ1gqQTx0MQc90O4QTkjPspm_5CcNSf0exSmUtNsnOTdUTLCAGTOP7RPkM9WXcBBFGgGr-_rEFK46I9vcUi";

        public AdsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Returns ads for user
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="adIdFrom">Id of ad from which all ads that have Id more than that considered as new</param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> GetAds(int userId, int adIdFrom)
        {
            try
            {
                var result = await _dbContext
              .Ads
              .Where(a => a.OwnerId == userId && a.Id > adIdFrom)
              .ToListAsync();

                if (result?.Count > 100)
                {
                    return Ok(result.TakeLast(100));
                }


                return Ok(result);
            }
            catch (Exception ex)
            {

                return Ok(ex.Message);
            }



        }

        [HttpPost]
        public async Task<IActionResult> PostAds([FromForm] int userId, [FromForm] string ads)
        {
            var postedAds = JsonConvert.DeserializeObject<List<Ad>>(ads);
            var addedList = new List<AdModel>();

            foreach (var item in postedAds)
            {
                var result = await _dbContext
                    .Ads
                    .Where(ad => 
                        ad.ProviderAdId == item.ProviderAdId && ad.OwnerId == item.OwnerId && ad.AdSource == item.AdSource)
                    .FirstOrDefaultAsync() ;

                if (result is null)
                {
                    _dbContext.Ads.Add(item);
                    addedList.Add((AdModel)item);
                }

            }

            var user = await _dbContext.Users.FindAsync(userId);

            if (addedList.Count == 0)
                return Ok();

            var task = MessagingService.SendPushNotificationWithData($"({addedList.Count()}) нових авто.","" ,new Random().Next(1, 9999999), user.MobileAppToken, _firebaseServerApiKey);

            _dbContext.SaveChanges();

            await Task.WhenAll(task);

            return Ok();
        }
    }
}