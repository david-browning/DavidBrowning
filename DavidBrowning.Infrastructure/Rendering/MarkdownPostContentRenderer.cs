// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using DavidBrowning.Models;
using DavidBrowning.Models.Publishing;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Infrastructure.Rendering;

public sealed class MarkdownPostContentRenderer
{
   public MarkdownPostContentRenderer(
      IMarkdownDocumentRenderer markdownRenderer)
   {
      _markdownRenderer = markdownRenderer;
   }

   public Task<RenderedContent> RenderAsync(
      PublishedTextContent content,
      CancellationToken cancellationToken = default)
   {
      if (content.ContentFormat != ContentFormat.Markdown &&
         content.ContentFormat != ContentFormat.PlainText)
      {
         throw new InvalidOperationException(
            $"Unsupported post content format: {content.ContentFormat}.");
      }

      var markdown = content.Content ?? throw new InvalidOperationException(
         $"PublishedTextContent does not contain content.");
      var references = content.AssetLinks.Select(link =>
      {
         return new LinkedAssetReference()
         {
            Caption = link.Caption,
            AltText = link.AltText,
            AssetKey = link.AssetKey,
            ReferenceKey = link.ReferenceKey,
         };
      }).ToList();

      return _markdownRenderer.RenderAsync(
         $"post-content:{content.CacheKey}",
         markdown, references, cancellationToken);
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

      var markdown = revision.Content ?? throw new InvalidOperationException(
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
         markdown, references, cancellationToken);
   }

   private readonly IMarkdownDocumentRenderer _markdownRenderer;
}