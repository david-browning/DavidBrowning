// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Services.Assets;
using Markdig;

namespace DavidBrowning.Services.Rendering;

public sealed partial class MarkdownDocumentRenderer :
   IMarkdownDocumentRenderer
{
   public MarkdownDocumentRenderer(IContentPipeline contentPipeline)
   {
      _contentPipeline = contentPipeline;

      _markdownPipeline = new MarkdownPipelineBuilder()
         .DisableHtml()
         .UseAdvancedExtensions()
         .Build();
   }

   public async Task<RenderedContent> RenderAsync(
      string documentKey,
      string markdown,
      IReadOnlyCollection<LinkedAssetReference> assetLinks,
      CancellationToken cancellationToken = default)
   {
      cancellationToken.ThrowIfCancellationRequested();

      var placeholders = new Dictionary<string, string>(
         StringComparer.OrdinalIgnoreCase);

      var renderedAssets = new Dictionary<string, string>();

      var referenceKeys = AssetTokenRegex()
         .Matches(markdown)
         .Select(match => match.Groups["key"].Value)
         .Distinct(StringComparer.OrdinalIgnoreCase)
         .ToList();

      foreach (var referenceKey in referenceKeys)
      {
         var assetLink = assetLinks.SingleOrDefault(link =>
            string.Equals(
               link.ReferenceKey,
               referenceKey,
               StringComparison.OrdinalIgnoreCase));

         if (assetLink is null)
         {
            throw new InvalidOperationException(
               $"Markdown document '{documentKey}' references missing " +
               $"asset '{referenceKey}'.");
         }

         ContentRenderOptions options = new()
         {
            AltText = assetLink.AltText,
            Caption = assetLink.Caption,
            CssClass = "wb-document-asset",
         };

         var renderedAsset =
            await _contentPipeline.GetRenderedContentAsync(
               assetLink.AssetKey,
               options,
               cancellationToken);

         var placeholder = $"%%ASSET_BLOCK_{placeholders.Count}%%";

         placeholders[referenceKey] = placeholder;
         renderedAssets[placeholder] = renderedAsset.Html;
      }

      var source = AssetTokenRegex().Replace(
         markdown,
         match => placeholders[match.Groups["key"].Value]);

      var html = Markdown.ToHtml(source, _markdownPipeline);

      foreach (var asset in renderedAssets)
      {
         html = html.Replace(
            $"<p>{asset.Key}</p>",
            asset.Value,
            StringComparison.Ordinal);
      }

      RenderedContent ret = new()
      {
         AssetKey = documentKey,
         OriginalContentType = _markdownContentType,
         Html = html,
      };

      return ret;
   }

   [GeneratedRegex(
      @"^\{\{asset:(?<key>[a-z0-9][a-z0-9-]*)\}\}\r?$",
      RegexOptions.Multiline |
      RegexOptions.IgnoreCase)]
   private static partial Regex AssetTokenRegex();

   private const string _markdownContentType = "text/markdown";

   private readonly IContentPipeline _contentPipeline;
   private readonly MarkdownPipeline _markdownPipeline;
}