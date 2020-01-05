using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdsAgregator.Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdsAgregator.Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SearchEngineController : ControllerBase
    {
        [HttpGet]
        public IActionResult Start()
        {
            SearchEngine.Start();
            return Ok(SearchEngine.GetEngineStatus().ToString());
        }

        [HttpGet]
        public IActionResult Stop()
        {
            SearchEngine.Stop();
            return Ok(SearchEngine.GetEngineStatus().ToString());
        }

        [HttpGet]
        public IActionResult GetStatus()
        {
            
            return Ok(SearchEngine.GetEngineStatus().ToString());
        }
    }
}