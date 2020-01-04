using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdsAgregator.Backend.Database;
using AdsAgregator.Backend.Database.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AdsAgregator.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchItemsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public SearchItemsController(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get([FromBody] int userId)
        {
            var searchItems = _dbContext
                .SearchItems
                .Where(si => si.OwnerId == userId);

            return Ok(JsonConvert.SerializeObject(searchItems));
        }

        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            var item = JsonConvert.DeserializeObject<SearchItem>(value);

            if (item == null)
                return StatusCode(500, "Cannot parse object");

            _dbContext.SearchItems.Add(item);
            await _dbContext.SaveChangesAsync();

            return Ok(JsonConvert.SerializeObject(item));
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] string value)
        {
            var item = JsonConvert.DeserializeObject<SearchItem>(value);

            if (item == null)
                return StatusCode(500, "Cannot parse object");


            var existingItem = await _dbContext.SearchItems.FindAsync(item.Id);

            if (existingItem == null)
                return StatusCode(400, "No item for update found");

            _dbContext.SearchItems.Update(item);
            await _dbContext.SaveChangesAsync();

            return Ok(JsonConvert.SerializeObject(item));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromBody] int userId, int itemId)
        {
            var itemToDelete = await _dbContext.SearchItems.FindAsync(itemId);

            if (itemToDelete == null) 
            {
                return StatusCode(400, "Cannot find search item with such id");
            }

            if (itemToDelete.OwnerId != userId)
            {
                return StatusCode(400, "User with such id is not owner of this search item");
            }

            _dbContext.SearchItems.Remove(itemToDelete);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
