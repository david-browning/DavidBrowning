// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Web.Controllers;
public sealed class SystemController : Controller
{
   [HttpGet("/system/warming-up")]
   [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
   public IActionResult WarmingUp([FromQuery] string? returnUrl)
   {
      if (string.IsNullOrWhiteSpace(returnUrl) ||
         !Url.IsLocalUrl(returnUrl))
      {
         returnUrl = "/";
      }

      return View("WarmingUp", returnUrl);
   }
}
