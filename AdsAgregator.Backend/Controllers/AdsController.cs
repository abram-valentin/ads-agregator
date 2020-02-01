using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdsAgregator.Backend.Database;
using AdsAgregator.Backend.Database.Tables;
using AdsAgregator.Backend.Services;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> PostAds([FromForm] int userId, [FromForm] List<Ad> ads)
        {
            var addedList = new List<Ad>();

            foreach (var item in ads)
            {
                var result = await _dbContext
                    .Ads
                    .Where(ad => 
                        ad.ProviderAdId == item.ProviderAdId && ad.OwnerId == item.OwnerId && ad.AdSource == item.AdSource)
                    .FirstOrDefaultAsync() ;

                if (result is null)
                {
                    _dbContext.Ads.Add(item);
                    addedList.Add(item);
                }

            }


            new DbLogger(_dbContext).Log(JsonConvert.SerializeObject(ads), DateTime.Now);

            _dbContext.SaveChanges();

            return Ok();
        }
    }
}