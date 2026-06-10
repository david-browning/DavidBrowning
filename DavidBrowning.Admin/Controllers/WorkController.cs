// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels.Work;
using DavidBrowning.Infrastructure.Assets;
using DavidBrowning.Infrastructure.Data.Stores;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public partial class WorkController : Controller
{
   public WorkController(
      IContentPipeline renderPipeline,
      IContentStore store,
      IWorkStore workStore)
   {
      _renderPipeline = renderPipeline;
      _workStore = workStore;
      _contentStore = store;
   }

   [HttpGet]
   public async Task<IActionResult> Index(
      CancellationToken cancellationToken)
   {
      return View(await GetIndexViewModelAsync(cancellationToken));
   }

   private async Task<IndexViewModel> GetIndexViewModelAsync(
      CancellationToken cancellationToken)
   {
      var credentials = await _workStore.GetCredentialsAsync(cancellationToken);
      var experiences = await _workStore.GetExperienceAsync(cancellationToken);

      return new IndexViewModel()
      {
         Resume = await GetResumeIndexViewModelAsync(cancellationToken),
         CredentialPreview = credentials.Select(
            c => new ViewModels.Work.Credentials.PreviewViewModel()
         {
               Id = c.Id,
               IssuingOrganization = c.IssuingOrganization,
               Name = c.Name,
         }).ToList(),
         ExperiencePreview = experiences.Select(
            e => new ViewModels.Work.Experience.PreviewViewModel()
         {
               CompanyName = e.CompanyName,
               Id = e.Id,
               Roles = e.Roles.ToList(),
         }).ToList(),
      };
   }

   private readonly IContentPipeline _renderPipeline;
   private readonly IWorkStore _workStore;
   private readonly IContentStore _contentStore;
}
