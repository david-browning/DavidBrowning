// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Infrastructure.Publishing;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public class PublishController : Controller
{
   public PublishController(IPublicSitePublisher publisher)
   {
      _publisher = publisher;
   }

   [HttpGet]
   public IActionResult Index()
   {
      return View();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Publish(
      CancellationToken cancellationToken)
   {
      try
      {
         var result = await _publisher.PublishAsync(cancellationToken);
         TempData["PublishStatus"] = $"Published version {result.Version}.";
         return RedirectToAction(nameof(Index));
      }
      catch (OperationCanceledException)
         when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception exception)
      {
         ModelState.AddModelError(
            string.Empty, $"Publication failed: {exception.Message}");

         return View("Index");
      }
   }

   private readonly IPublicSitePublisher _publisher;
}
