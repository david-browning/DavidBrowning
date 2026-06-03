// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Models.Writing;
using DavidBrowning.Services.Assets;
using Markdig;

namespace DavidBrowning.Services.Rendering;

public sealed partial class MarkdownPostContentRenderer : IPostContentRenderer
{
   public MarkdownPostContentRenderer(IContentPipeline content)
   {
      _pipeline = content;
      _markdown = new MarkdownPipelineBuilder()
         .DisableHtml()
         .UseAdvancedExtensions()
         .Build();
   }

   public async Task<RenderedContent> RenderAsync(
      PostRevision revision,
      IReadOnlyCollection<PostAssetLink> assetLinks,
      CancellationToken cancellationToken = default)
   {
      cancellationToken.ThrowIfCancellationRequested();
      if (revision.ContentFormat != ContentFormat.Markdown)
      {
         throw new InvalidOperationException(
            $"Unsupported post content format: {revision.ContentFormat}.");
      }

      var source = revision.Content ??
         throw new InvalidOperationException(
            $"Post revision {revision.Id} does not contain content.");

      var replacements = new Dictionary<string, string>();

      foreach (Match match in AssetTokenRegex().Matches(source))
      {
         var referenceKey = match.Groups["key"].Value;
         var link = assetLinks.SingleOrDefault(link =>
            string.Equals(
               link.ReferenceKey,
               referenceKey,
               StringComparison.OrdinalIgnoreCase));

         if (link is null)
         {
            throw new InvalidOperationException(
               $"Post revision {revision.Id} references missing asset " +
               $"'{referenceKey}'.");
         }

         var asset = link.SiteAsset ?? throw new InvalidOperationException(
            $"Post asset link '{referenceKey}' is missing its asset.");

         var placeholder = $"%%ASSET_BLOCK_{replacements.Count}%%";

         ContentRenderOptions options = new()
         {
            AltText = link.AltTextOverride ?? asset.AltText,
            Caption = link.Caption,
            CssClass = "wb-post-asset",
         };

         var renderedAsset =
            await _pipeline.GetRenderedContentAsync(
               asset.AssetKey, options, cancellationToken);
         if(renderedAsset == null)
         {
            throw new InvalidOperationException(
               $"Could not render {asset.AssetKey}");
         }

         replacements[placeholder] = renderedAsset.Html;
         source = source.Replace(
            match.Value, placeholder, StringComparison.Ordinal);
      }

      var html = Markdown.ToHtml(source, _markdown);
      foreach (var replacement in replacements)
      {
         html = html.Replace(
            $"<p>{replacement.Key}</p>",
            replacement.Value,
            StringComparison.Ordinal);
      }

      return new()
      {
         AssetKey = $"post-revision:{revision.Id}",
         OriginalContentType = "text/markdown",
         Html = html,
      };
   }

   private readonly IContentPipeline _pipeline;
   private readonly MarkdownPipeline _markdown;
   [GeneratedRegex(
      @"^\{\{asset:(?<key>[a-z0-9][a-z0-9-]*)\}\}$",
      RegexOptions.Multiline)]
   private static partial Regex AssetTokenRegex();
}
