// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Extensions;
using DavidBrowning.Models.Projects;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Services.Assets;

namespace DavidBrowning.Services.Rendering;

public sealed class MarkdownProjectContentRenderer
{
   public MarkdownProjectContentRenderer(
      IContentStore contentStore,
      IMarkdownDocumentRenderer markdownRenderer)
   {
      _contentStore = contentStore;
      _markdownRenderer = markdownRenderer;
   }

   public async Task<RenderedContent> RenderAsync(
      Project project,
      CancellationToken cancellationToken = default)
   {
      var contentLink = project.AssetLinks.SingleOrDefault(link =>
         link.ProjectAssetRole?.Slug.EqualsOrdinalIgnoreCase(
            _detailsContentRoleSlug) == true);

      if (contentLink is null)
      {
         throw new InvalidOperationException(
            $"Project '{project.Slug}' does not have a linked " +
            $"'{_detailsContentRoleSlug}' asset.");
      }

      var contentAsset = contentLink.SiteAsset ??
         throw new InvalidOperationException(
            "Project details content is missing its site asset.");

      var storedAsset = await _contentStore.GetAssetAsync(
         contentAsset.AssetKey,
         cancellationToken);

      var contentType = AssetHelpers.GetMediaType(
         storedAsset.ContentType);

      if (!contentType.EqualsOrdinalIgnoreCase(_markdownContentType))
      {
         throw new InvalidOperationException(
            $"Project details asset '{contentAsset.AssetKey}' must use " +
            $"content type '{_markdownContentType}'.");
      }

      var markdown = storedAsset.Text ??
         throw new InvalidOperationException(
            $"Project details asset '{contentAsset.AssetKey}' does not " +
            "contain text.");

      var references = project.AssetLinks
         .Where(link => !string.IsNullOrWhiteSpace(link.ReferenceKey))
         .Select(link =>
         {
            var asset = link.SiteAsset ??
               throw new InvalidOperationException(
                  $"Project '{project.Slug}' contains linked asset " +
                  $"'{link.ReferenceKey}', but its SiteAsset navigation property " +
                  "was not loaded.");

            return new LinkedAssetReference()
            {
               ReferenceKey = link.ReferenceKey!,
               AssetKey = asset.AssetKey,
               AltText = link.AltTextOverride ?? asset.AltText,
               Caption = link.Caption,
            };
         })
         .ToList();

      return await _markdownRenderer.RenderAsync(
         contentAsset.AssetKey,
         markdown,
         references,
         cancellationToken);
   }

   private const string _detailsContentRoleSlug = "details-content";
   private const string _markdownContentType = "text/markdown";

   private readonly IContentStore _contentStore;
   private readonly IMarkdownDocumentRenderer _markdownRenderer;
}