// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Services.Rendering;

public sealed class MarkdownPostContentRenderer
{
   public MarkdownPostContentRenderer(
      IMarkdownDocumentRenderer markdownRenderer)
   {
      _markdownRenderer = markdownRenderer;
   }

   public Task<RenderedContent> RenderAsync(
      PostRevision revision,
      IReadOnlyCollection<PostRevisionAssetLink> assetLinks,
      CancellationToken cancellationToken = default)
   {
      if (revision.ContentFormat != ContentFormat.Markdown)
      {
         throw new InvalidOperationException(
            $"Unsupported post content format: {revision.ContentFormat}.");
      }

      var markdown = revision.Content ??
         throw new InvalidOperationException(
            $"Post revision {revision.Id} does not contain content.");

      var references = assetLinks
         .Select(link =>
         {
            var asset = link.SiteAsset ??
               throw new InvalidOperationException(
                  $"Post revision {revision.Id} contains linked asset " +
                  $"'{link.ReferenceKey}', but its SiteAsset navigation property " +
                  "was not loaded.");

            return new LinkedAssetReference()
            {
               ReferenceKey = link.ReferenceKey,
               AssetKey = asset.AssetKey,
               AltText = link.AltTextOverride ?? asset.AltText,
               Caption = link.Caption,
            };
         })
         .ToList();

      return _markdownRenderer.RenderAsync(
         $"post-revision:{revision.Id}",
         markdown,
         references,
         cancellationToken);
   }

   private readonly IMarkdownDocumentRenderer _markdownRenderer;
}