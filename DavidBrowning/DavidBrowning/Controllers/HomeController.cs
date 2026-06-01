// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Data.Stores.Projects;
using DavidBrowning.Data.Stores.Writing;
using DavidBrowning.Diagnostics;
using DavidBrowning.Models.ViewModels.Home;
using DavidBrowning.Services.Cache;
using DavidBrowning.Services.Time;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Controllers;

public class HomeController : Controller
{
   public HomeController(
      ILogger<HomeController> logger,
      ISystemClock clock,
      IErrorStore errorLogStore,
      IOptions<DiagnosticsOptions> options,
      IWebHostEnvironment environment,
      IConfiguration configuration,

      IProjectStore projectStore,
      IWritingStore writingStore,
      JsonCache jsonCache)
   {
      _logger = logger;
      _clock = clock;
      _errorLogStore = errorLogStore;
      _options = options.Value;
      _webHostEnvironment = environment;
      _configuration = configuration;

      _projectStore = projectStore;
      _writingStore = writingStore;
      _jsonCache = jsonCache;
   }

   public IActionResult Index()
   {
      return View();
   }

   public async Task<IActionResult> Privacy(CancellationToken cancellationToken)
   {
      return View(await GetPrivacyModelAsync(cancellationToken));
   }

   private async Task<PrivacyViewModel> GetPrivacyModelAsync(
      CancellationToken cancellationToken)
   {
      var data = await _jsonCache.GetJsonFileContentAsync<PrivacyViewModel>(
         "Heros/Privacy.json", cancellationToken);
      if (data == null) {
         throw new FileNotFoundException("Missing privacy data");
      }

      return data;
   }

   private readonly ILogger<HomeController> _logger;
   private readonly ISystemClock _clock;
   private readonly IErrorStore _errorLogStore;
   private readonly DiagnosticsOptions _options;
   private readonly IWebHostEnvironment _webHostEnvironment;
   private readonly IConfiguration _configuration;

   private readonly IProjectStore _projectStore;
   private readonly IWritingStore _writingStore;
   private readonly JsonCache _jsonCache;
}
