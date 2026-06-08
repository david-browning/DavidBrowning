// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Admin.Controllers;
public class HomeController : Controller
{
   private readonly ILogger<HomeController> _logger;

   public HomeController(ILogger<HomeController> logger)
   {
      _logger = logger;
   }

   public IActionResult Index()
   {
      return View();
   }

   public IActionResult Privacy()
   {
      return View();
   }
}
