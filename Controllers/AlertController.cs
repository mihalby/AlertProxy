using AlertProxy.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertProxy.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]/{*catchall}")]
    public class AlertController : ControllerBase
    {
        
        private readonly ILogger<AlertController> _logger;
        private readonly IConfiguration _config;
        private readonly IProxy _proxy;

        public AlertController(ILogger<AlertController> logger,IConfiguration config, IProxy proxy)
        {
            _logger = logger;
            _config = config;
            _proxy = proxy;
        }

        

        [HttpPost]
        public async Task<IActionResult> PostAsync(object alert)
        {
            
            var ret = await _proxy.Process(RouteData.Values["catchall"].ToString(), alert);

            return Ok(ret);
        }
    }
}
