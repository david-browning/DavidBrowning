// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Controllers;

public class Workbench : Controller
{
   public IActionResult Index()
   {
      return View();
   }
}
