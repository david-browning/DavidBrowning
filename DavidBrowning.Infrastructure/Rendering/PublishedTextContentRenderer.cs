// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using DavidBrowning.Models;
using DavidBrowning.Models.Publishing;

namespace DavidBrowning.Infrastructure.Rendering;

public sealed class PublishedTextContentRenderer
{
   public PublishedTextContentRenderer(
      IMarkdownDocumentRenderer markdownRenderer)
   {
      _markdownRenderer = markdownRenderer;
   }

   public Task<RenderedContent> RenderAsync(
      PublishedTextContent content,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(content);

      if (content.ContentFormat != ContentFormat.Markdown &&
         content.ContentFormat != ContentFormat.PlainText)
      {
         throw new InvalidOperationException(
            $"Unsupported published content format: " +
            $"'{content.ContentFormat}'.");
      }

      var markdown = content.Content ??
         throw new InvalidOperationException(
            $"Published content '{content.CacheKey}' does not contain text.");

      var references = content.AssetLinks
         .Select(link => new LinkedAssetReference()
         {
            ReferenceKey = link.ReferenceKey,
            AssetKey = link.AssetKey,
            AltText = link.AltText,
            Caption = link.Caption,
         })
         .ToArray();

      return _markdownRenderer.RenderAsync(
         content.CacheKey,
         markdown,
         references,
         cancellationToken);
   }

   private readonly IMarkdownDocumentRenderer _markdownRenderer;
}