using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WOTCH.Interfaces;
using WOTCH.Lib;
using WOTCH.Models;

namespace WOTCH.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRequestHandler _requestHandler;

        public HomeController(ILogger<HomeController> logger, IRequestHandler requestHandler)
        {
            _logger = logger;
            _requestHandler = requestHandler;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetData([FromBody]object data)
        {
            _requestHandler.Proceed(data);
            return StatusCode(200);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
