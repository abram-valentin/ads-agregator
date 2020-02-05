using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdsAgregator.Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EfficientDelivery_OrdersController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostOrders([FromForm] int userId, [FromForm] string orders)
        {
            throw new NotImplementedException();
        }
    }
}