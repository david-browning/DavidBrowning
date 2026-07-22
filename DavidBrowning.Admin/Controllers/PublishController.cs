// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels.Publish;
using DavidBrowning.Infrastructure.Publishing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Admin.Controllers;

[Authorize]
public sealed class PublishController : Controller
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
      var model = new IndexViewModel()
      {
         ResultMessage = TempData[_resultMessageTempDataKey] as string,
      };

      if (TempData[_resultStatusTempDataKey] is string statusText &&
         Enum.TryParse(
            statusText,
            ignoreCase: true,
            out PublishResultStatus status))
      {
         model.ResultStatus = status;
      }

      return View(model);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Publish(
      CancellationToken cancellationToken)
   {
      try
      {
         var result = await _publisher.PublishAsync(cancellationToken);

         SetResult(
            $"Published version {result.Version}.",
            PublishResultStatus.Success);
      }
      catch (OperationCanceledException)
         when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception exception)
      {
         _logger.LogError(exception, "Public site publication failed.");

         SetResult(
            "Publication failed. Review the application logs for details.",
            PublishResultStatus.Error);
      }

      return RedirectToAction(nameof(Index));
   }

   private void SetResult(string message, PublishResultStatus status)
   {
      TempData[_resultMessageTempDataKey] = message;
      TempData[_resultStatusTempDataKey] = status.ToString();
   }

   private const string _resultMessageTempDataKey = "PublishResultMessage";
   private const string _resultStatusTempDataKey = "PublishResultStatus";
   private readonly IPublicSitePublisher _publisher;
   private readonly ILogger<PublishController> _logger;
}