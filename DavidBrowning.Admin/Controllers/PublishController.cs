// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Infrastructure.Publishing;
using DavidBrowning.Admin.ViewModels.Publish;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Admin.Controllers;

public class PublishController : Controller
{
   public PublishController(
      IPublicSitePublisher publisher,
      ILogger<PublishController> logger)
   {
      _publisher = publisher;
      _logger = logger;
   }

   [HttpGet]
   public IActionResult Index()
   {
      return View(new IndexViewModel());
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Publish(
      CancellationToken cancellationToken)
   {
      string resultMessage;
      try
      {
         var result = await _publisher.PublishAsync(cancellationToken);
         resultMessage = $"Published version {result.Version}.";
      }
      catch (OperationCanceledException)
         when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception exception)
      {
         resultMessage = $"Publication failed: {exception.Message}";
         _logger.LogError(exception, "Public site publication failed.");
         ModelState.AddModelError("Result-Exception", resultMessage);
      }

      return View("Index", new IndexViewModel()
      {
         ResultMessage = resultMessage,
      });
   }

   private readonly IPublicSitePublisher _publisher;
   private readonly ILogger<PublishController> _logger;
}
