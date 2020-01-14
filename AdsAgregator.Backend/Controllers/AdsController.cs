using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdsAgregator.Backend.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var result = await _dbContext
                .Ads
                .Where(a => a.OwnerId == userId && a.Id > adIdFrom)
                .ToListAsync();

            return Ok(result);
        }
    }
}