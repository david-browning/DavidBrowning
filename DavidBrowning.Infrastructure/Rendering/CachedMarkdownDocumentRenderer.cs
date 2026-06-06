// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models;
using DavidBrowning.Services.Cache;

namespace DavidBrowning.Services.Rendering;

public sealed class CachedMarkdownDocumentRenderer :
   IMarkdownDocumentRenderer
{
   private readonly IMarkdownDocumentRenderer _innerRenderer;
   private readonly RenderedContentMemoryCache _cache;

   public CachedMarkdownDocumentRenderer(
      IMarkdownDocumentRenderer innerRenderer,
      RenderedContentMemoryCache cache)
   {
      _innerRenderer = innerRenderer;
      _cache = cache;
   }

   public Task<RenderedContent> RenderAsync(
      string documentKey,
      string markdown,
      IReadOnlyCollection<LinkedAssetReference> assetLinks,
      CancellationToken cancellationToken = default)
   {
      var cacheKey = GetCacheKey(documentKey, markdown, assetLinks);
      return _cache.GetOrCreateAsync(
         cacheKey,
         token => _innerRenderer.RenderAsync(
            documentKey,
            markdown,
            assetLinks,
            token),
         cancellationToken);
   }

   private static string GetCacheKey(
      string documentKey,
      string markdown,
      IReadOnlyCollection<LinkedAssetReference> assetLinks)
   {
      var source = new StringBuilder();

      AppendPart(source, "renderer-version", _rendererVersion);
      AppendPart(source, "document-key", documentKey);
      AppendPart(source, "markdown", markdown);

      var orderedAssetLinks = assetLinks
         .OrderBy(
            link => link.ReferenceKey,
            StringComparer.OrdinalIgnoreCase)
         .ThenBy(
            link => link.AssetKey,
            StringComparer.Ordinal)
         .ToList();

      AppendPart(
         source,
         "asset-link-count",
         orderedAssetLinks.Count.ToString(
            CultureInfo.InvariantCulture));

      foreach (var assetLink in orderedAssetLinks)
      {
         AppendPart(
            source,
            "reference-key",
            assetLink.ReferenceKey.ToLowerInvariant());

         AppendPart(source, "asset-key", assetLink.AssetKey);
         AppendPart(source, "alt-text", assetLink.AltText);
         AppendPart(source, "caption", assetLink.Caption);
      }

      var sourceBytes = Encoding.UTF8.GetBytes(source.ToString());
      var hashBytes = SHA256.HashData(sourceBytes);
      var hash = Convert.ToHexString(hashBytes).ToLowerInvariant();

      return $"markdown-document:{hash}";
   }

   private static void AppendPart(
      StringBuilder builder,
      string name,
      string? value)
   {
      builder.Append(name);
      builder.Append(':');

      if (value is null)
      {
         builder.Append("-1:");
         return;
      }

      builder.Append(
         value.Length.ToString(CultureInfo.InvariantCulture));

      builder.Append(':');
      builder.Append(value);
      builder.Append(';');
   }

   private const string _rendererVersion = "v1";
}