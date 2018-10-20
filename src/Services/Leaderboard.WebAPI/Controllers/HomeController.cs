using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Prometheus.Client;
using Prometheus.Client.Collectors;

namespace Leaderboard.WebAPI.Controllers
{
    [SwaggerIgnore]
    public class HomeController : Controller
    {        
        public IActionResult Index()
        {
            var counter= Metrics.CreateCounter("homecontroller_request_counter", "Counts number of requests on home controller", "count");
            counter.Inc();

            return new RedirectResult("~/swagger");
        }
    }
}
