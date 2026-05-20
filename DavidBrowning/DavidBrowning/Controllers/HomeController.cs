// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Diagnostics;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Writing;
using DavidBrowning.Models;
using DavidBrowning.Models.Projects;
using DavidBrowning.Services.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Controllers
{
   public class HomeController : Controller
   {
      public HomeController(
         ILogger<HomeController> logger,
         IWritingStore writingStore,
         ISlugLookupService<ProjectVisibility> projectLookup)
      {
         _logger = logger;
         _writingStore = writingStore;
         _projectVisibilityLookup = projectLookup;
      }

      public async Task<IActionResult> Index()
      {
         //int? publishedVisibilityId = await _projectVisibilityLookup.GetIdBySlugAsync("published");
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
      private readonly ISlugLookupService<ProjectVisibility> _projectVisibilityLookup;
   }
}
