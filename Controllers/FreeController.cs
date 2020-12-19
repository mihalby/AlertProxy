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
    public class FreeController : ControllerBase
    {

        private readonly ILogger<AlertController> _logger;
        private readonly IConfiguration _config;
        private readonly IProxy _proxy;

        public FreeController(ILogger<AlertController> logger, IConfiguration config, IProxy proxy)
        {
            _logger = logger;
            _config = config;
            _proxy = proxy;
        }



        [HttpPost]
        public async Task<IActionResult> PostAsync(object obj)
        {

            var ret = await _proxy.ProcessFree(RouteData.Values["catchall"].ToString(), obj);

            return Ok(ret.StatusCode);
        }
    }
}
