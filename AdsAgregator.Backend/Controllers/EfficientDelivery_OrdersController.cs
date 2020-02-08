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
        private const string _firebaseServerApiKey = "AAAAIVvHwY4:APA91bGTYX799f9qndT6YN5AbQN3W4UoRZbg8lsj-FSp8s8CsyJ65nSfNzx6DTPvDLNCTmDgfrQktxktfnZP7i7anbwBOoX1FQw072bwQkVrBjK4ceLpKDHwnp_LwECbEtP05kxxl60O";

        public EfficientDelivery_OrdersController(EfficientDeliveryDbContext deliveryDbContext)
        {
            _dbContext = deliveryDbContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetOrders(int userId, int adIdFrom)
        {
            try
            {
                var result = _dbContext
              .Orders
              .OrderByDescending(o => o.PublishDate)
              .Take(100);


                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

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

                    if (result != null)
                    {
                        result.PublishDate = item.PublishDate;
                        _dbContext.Orders.Update(result);
                    }

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

                var task = MessagingService.SendPushNotificationWithData($"({addedOrders.Count()}) нових заявок.", "", new Random().Next(1, 9999999), user.MobileAppToken, _firebaseServerApiKey);

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