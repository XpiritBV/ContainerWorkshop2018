﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Leaderboard.WebAPI.Controllers
{
    [SwaggerIgnore]
    public class HomeController : Controller
    {        
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
