// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Services.Rendering;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Services.Assets;

public class BasicContentPipeline : IContentPipeline
{
   public BasicContentPipeline(
      ILogger<BasicContentPipeline> logger,
      IContentService contentService,
      IContentRenderer contentRenderer)
   {
      _logger = logger;
      _contentRenderer = contentRenderer;
      _contentStore = contentService;
   }

   public async Task<RenderedContent> GetRenderedContentAsync(
      string assetKey,
      ContentRenderOptions? options = null,
      CancellationToken cancellationToken = default)
   {
      var asset = await _contentStore.GetAssetAsync(
         assetKey, cancellationToken);
      var rendered = await _contentRenderer.RenderAsync(
         asset, options, cancellationToken);
      return rendered;
   }

   public async Task<T> GetJsonFileContentAsync<T>(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      var asset = await _contentStore.GetAssetAsync(
       assetKey, cancellationToken);
      if (asset.SourceFormat != ContentSourceFormat.Json)
      {
         throw new InvalidOperationException(
            $"{assetKey} is not a JSON file.");
      }

      if (asset.Text == null)
      {
         throw new InvalidOperationException(
            $"JSON asset {assetKey} does not contain any text.");
      }

      var model = JsonSerializer.Deserialize<T>(
         asset.Text,
         new JsonSerializerOptions()
         {
            PropertyNameCaseInsensitive = true,
         });

      if (model == null)
      {
         throw new InvalidOperationException(
            $"JSON asset {assetKey} could not be parsed as {typeof(T).Name}.");
      }

      return model;
   }

   private readonly ILogger<BasicContentPipeline> _logger;
   private readonly IContentService _contentStore;
   private readonly IContentRenderer _contentRenderer;
}