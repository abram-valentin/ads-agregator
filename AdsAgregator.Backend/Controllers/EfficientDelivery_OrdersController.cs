using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdsAgregator.Backend.Services;
using EfficientDelivery.DAL.Database;
using EfficientDelivery.DAL.Database.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AdsAgregator.Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EfficientDelivery_OrdersController : ControllerBase
    {
        public EfficientDeliveryDbContext _dbContext { get; set; }

        public EfficientDelivery_OrdersController(EfficientDeliveryDbContext deliveryDbContext)
        {
            _dbContext = deliveryDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostOrders([FromForm] int userId, [FromForm] string ordersJson)
        {
            try
            {
                var orders = JsonConvert.DeserializeObject<List<EfficientDelivery.CommonModels.Order>>(ordersJson);


                var addedOrders = new List<Order>();

                foreach (var item in orders)
                {
                    var result = await _dbContext
                        .Orders
                        .Where(ad =>
                            ad.OrderId == item.OrderId && ad.OwnerId == userId && ad.OrderSource == item.OrderSource)
                        .FirstOrDefaultAsync();

                    if (result is null)
                    {
                        var order = new EfficientDelivery.DAL.Database.Tables.Order
                        {
                            OwnerId = userId,
                            OrderId = item.OrderId,
                            OrderSource = item.OrderSource,
                            DistanceInfo = item.DistanceInfo,
                            PublishDate = item.PublishDate,
                            CargoShippingDateInfo = item.CargoShippingDateInfo,
                            TransportType = item.TransportType,
                            LocationFrom = item.LocationFrom,
                            LocationTo = item.LocationTo,
                            CargoInfo = item.CargoInfo,
                            PaymentInfo = item.PaymentInfo,
                            OrderLink = item.OrderLink,
                        };

                        _dbContext.Orders.Add(order);
                        addedOrders.Add(order);
                    }

                }

                var user = await _dbContext.Users.FindAsync(userId);

                if (user is null)
                     return StatusCode(500, "user not found");

                if (addedOrders.Count == 0)
                    return Ok();

                var task = MessagingService.SendPushNotificationWithData($"({addedOrders.Count()}) нових заявок.", "", new Random().Next(1, 9999999), user.MobileAppToken);

                _dbContext.SaveChanges();

                await Task.WhenAll(task);

                return Ok();
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
         
        }
    }
}