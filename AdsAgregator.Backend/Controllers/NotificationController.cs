using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdsAgregator.Backend.Database;
using AdsAgregator.Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdsAgregator.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        public AppDbContext _database{ get; set; }
        public NotificationController(AppDbContext database)
        {
            _database = database;
        }

        [HttpGet]
        public async Task<IActionResult> Notify(string title, string message)
        {
            //var messagingService = new MessagingService();
            //var token = "efT-uqpzoqY:APA91bFhlMEUswAA1wwWHkD1wXfyKRHuYUANLe_0z6TCU7ghPEnjGjB9m1Z8xIyggAODh1OFgyVwRKeo4UuoLqy0hqM73fUoUq_YGz2Nbhmzj2DR58C1fxyWHLACifBK6-Pgcz1dPosT";
            //await messagingService.SendPushNotificationWithData(title, message, "test message", token );

            return Ok();
        }
    }
}