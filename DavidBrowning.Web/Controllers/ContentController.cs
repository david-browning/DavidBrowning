// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Infrastructure.Assets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace DavidBrowning.Web.Controllers;

[Route("content")]
public sealed class ContentController : Controller
{
   public ContentController(
      IWebHostEnvironment environment,
      IContentStore contentStore)
   {
      _environment = environment;
      _contentStore = contentStore;
   }

   [HttpGet("{**assetKey}", Name = "GetContentAsset")]
   public async Task<IActionResult> GetAsync(
      string assetKey,
      CancellationToken cancellationToken)
   {
      try
      {
         if (_environment.IsDevelopment())
         {
            Response.Headers.CacheControl = "no-cache";
         }
         else
         {
            Response.Headers.CacheControl = "public, max-age=3600";
         }

         var asset = await _contentStore.GetAssetAsync(
            assetKey, cancellationToken);

         var stream = await _contentStore.OpenReadAsync(
            assetKey, cancellationToken);

         return File(
            fileStream: stream,
            contentType: asset.ContentType,
            lastModified: asset.LastModifiedUtc,
            entityTag: new EntityTagHeaderValue(asset.EntityTag));
      }
      catch (FileNotFoundException)
      {
         return NotFound();
      }
      catch (ArgumentException)
      {
         return NotFound();
      }
   }

   private readonly IWebHostEnvironment _environment;
   private readonly IContentStore _contentStore;
}