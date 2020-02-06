using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfficientDelivery.DAL.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AdsAgregator.Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EfficientDelivery_SearchItemsController : ControllerBase
    {
        private readonly EfficientDeliveryDbContext _dbContext;

        public EfficientDelivery_SearchItemsController(EfficientDeliveryDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get(int userId)
        {
            var searchItems = _dbContext
                .Searches
                .Where(si => si.OwnerId == userId);

            return Ok(searchItems);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromForm]int userId, [FromForm] string value)
        {
            var item = JsonConvert.DeserializeObject<EfficientDelivery.DAL.Database.Tables.SearchItem>(value);

            if (item == null)
                return StatusCode(500, "Cannot parse object");

            item.OwnerId = userId;

            _dbContext.Searches.Add(item);
            await _dbContext.SaveChangesAsync();

            return Ok(item);
        }


        [HttpPost]
        public async Task<IActionResult> Update([FromForm] int userId, [FromForm] string value)
        {
            var item = JsonConvert.DeserializeObject<EfficientDelivery.DAL.Database.Tables.SearchItem>(value);

            if (item == null)
                return StatusCode(500, "Cannot parse object");


            var existingItem = await _dbContext.Searches.FindAsync(item.Id);

            if (existingItem == null)
                return StatusCode(400, "No item for update found");

            existingItem.Title = item.Title;
            existingItem.Description = item.Description;
            existingItem.IsActive = item.IsActive;
            existingItem.Url = item.Url;


            _dbContext.Searches.Update(existingItem);
            await _dbContext.SaveChangesAsync();

            return Ok(existingItem);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int userId, [FromForm] int itemId)
        {
            var itemToDelete = await _dbContext.Searches.FindAsync(itemId);

            if (itemToDelete == null)
            {
                return StatusCode(400, "Cannot find search item with such id");
            }

            if (itemToDelete.OwnerId != userId)
            {
                return StatusCode(400, "User with such id is not owner of this search item");
            }

            _dbContext.Searches.Remove(itemToDelete);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}