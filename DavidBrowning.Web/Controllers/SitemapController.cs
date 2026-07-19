// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Infrastructure;
using DavidBrowning.Infrastructure.Cache;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.Infrastructure.Seo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Hosting;

namespace DavidBrowning.Web.Controllers;

[OutputCache(PolicyName = PolicyNames.SiteMap)]
public class SitemapController : Controller
{
   public SitemapController(
      IHostEnvironment hostEnvironment,
      UrlBuilder urlBuilder,
      JsonCache jsonCache,
      SitemapBuilder sitemap,
      IProjectStore projectStore,
      IWritingStore writingStore)
   {
      _hostEnvironment = hostEnvironment;
      _urlBuilder = urlBuilder;
      _sitemapBuilder = sitemap;
      _projectStore = projectStore;
      _writingStore = writingStore;
      _jsonCache = jsonCache;
   }

   [HttpGet("/sitemap.xml")]
   public async Task<IActionResult> Index(CancellationToken cancellationToken)
   {
      if (_hostEnvironment.IsDevelopment())
      {
         Response.Headers.CacheControl = "no-store, no-cache";
      }
      else
      {
         Response.Headers.CacheControl = "public, max-age=3600";
      }

      // Get the public pages.
      var publicPages =
         await _jsonCache.GetJsonFileContentAsync<List<SitemapEntry>>(
            "PublicPages.json");

      // Duplicate the entries because a reference to the stored pages is 
      // returned
      var entries = publicPages.ToList();

      // Add published writings and projects
      var posts = await _writingStore.GetPublishedPostsAsync(cancellationToken);
      entries.AddRange(posts.Select(post => new SitemapEntry()
      {
         RelativePath = _urlBuilder.GetDetailsUrl(post.Slug, "writing"),
         LastModifiedUtc = post.LastUpdatedDateUtc,
      }));

      var projects = await _projectStore.GetPublishedProjectsAsync(
         cancellationToken);
      entries.AddRange(projects.Select(project => new SitemapEntry()
      {
         RelativePath = _urlBuilder.GetDetailsUrl(project.Slug, "projects"),
         LastModifiedUtc = project.UpdatedAtUtc,
      }));

      // Add all the project tags and stacks
      var projectTags = await _projectStore.GetProjectTagsAsync(
         cancellationToken);
      entries.AddRange(projectTags.Select(tag => new SitemapEntry()
      {
         RelativePath = _urlBuilder.GetFilterUrl(tag.Slug, "projects", "tags"),
      }));

      var projectStacks = await _projectStore.GetProjectStackTagsAsync(
         cancellationToken);
      entries.AddRange(projectStacks.Select(stack => new SitemapEntry()
      {
         RelativePath = _urlBuilder.GetFilterUrl(
            stack.Slug, "projects", "stacks"),
      }));

      // Add all the writing tags
      var postTags = await _writingStore.GetTagsAsync(cancellationToken);
      entries.AddRange(postTags.Select(tag => new SitemapEntry()
      {
         RelativePath = _urlBuilder.GetFilterUrl(tag.Slug, "writing", "tags"),
      }));

      var xml = _sitemapBuilder.GenerateXml(entries);
      return Content(xml, "application/xml", Encoding.UTF8);
   }

   private readonly IHostEnvironment _hostEnvironment;
   private readonly IProjectStore _projectStore;
   private readonly IWritingStore _writingStore;
   private readonly SitemapBuilder _sitemapBuilder;
   private readonly JsonCache _jsonCache;
   private readonly UrlBuilder _urlBuilder;
}
