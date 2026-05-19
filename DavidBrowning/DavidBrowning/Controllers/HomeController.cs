// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Writing;
using DavidBrowning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Controllers
{
   public class HomeController : Controller
   {
      public HomeController(
         ILogger<HomeController> logger,
         IWritingStore writingStore)
      {
         _logger = logger;
         _writingStore = writingStore;
      }

      public IActionResult Index()
      {
         return View();
      }

      public IActionResult Privacy()
      {
         return View();
      }

      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error()
      {
         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }

      private readonly ILogger<HomeController> _logger;
      private readonly IWritingStore _writingStore;
   }
}
