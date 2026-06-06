// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Net;
using DavidBrowning.Helpers;
using DavidBrowning.Infrastructure.Assets;
using DavidBrowning.Models;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Infrastructure.Rendering;

public sealed class BasicContentRenderer : IContentRenderer
{
   public BasicContentRenderer(ILogger<BasicContentRenderer> logger)
   {
      _logger = logger;
   }

   public Task<RenderedContent> RenderAsync(
      StoredAsset content,
      ContentRenderOptions? options = null,
      CancellationToken cancellationToken = default)
   {
      cancellationToken.ThrowIfCancellationRequested();
      var contentType = AssetHelpers.GetMediaType(content.ContentType);
      string html;
      if (contentType.EqualsOrdinalIgnoreCase(_htmlContentType))
      {
         html = GetHtml(content);
      }
      else if (contentType.EqualsOrdinalIgnoreCase(_plainTextContentType))
      {
         html = GetTextHtml(content);
      }
      else if (contentType.EqualsOrdinalIgnoreCase(_markdownContentType))
      {
         html = GetMarkdownHtml(content);
      }
      else if (contentType.StartsWith(
         _imageContentTypePrefix,
         StringComparison.OrdinalIgnoreCase))
      {
         html = GetImageHtml(content, options);
      }
      else
      {
         throw new InvalidOperationException(
            $"Unsupported asset content type: {content.ContentType}.");
      }

      RenderedContent ret = new()
      {
         AssetKey = content.AssetKey,
         OriginalContentType = content.ContentType,
         Html = html,
      };

      return Task.FromResult(ret);
   }

   private static string GetHtml(StoredAsset content)
   {
      // TODO: Sanitize the HTML.
      throw new InvalidOperationException(
         "Raw HTML content is not supported yet.");
   }

   private static string GetMarkdownHtml(StoredAsset content)
   {
      // TODO: Render Markdown and sanitize the resulting HTML.
      throw new InvalidOperationException(
         "Markdown content is not supported yet.");
   }

   private static string GetTextHtml(StoredAsset content)
   {
      return ParagraphizeHtml(GetAssetText(content));
   }

   private string GetImageHtml(
      StoredAsset content,
      ContentRenderOptions? options)
   {
      if (options?.AltText is null)
      {
         _logger.LogWarning(
            "Image asset {AssetKey} does not have alt text.",
            content.AssetKey);
      }

      var src = WebUtility.HtmlEncode(GetAssetUrl(content.AssetKey));
      var altText = WebUtility.HtmlEncode(options?.AltText ?? string.Empty);
      var cssAttribute = string.Empty;
      if (!string.IsNullOrWhiteSpace(options?.CssClass))
      {
         var cssClass = WebUtility.HtmlEncode(options.CssClass);
         cssAttribute = $" class=\"{cssClass}\"";
      }

      return
         $"<img src=\"{src}\" alt=\"{altText}\"{cssAttribute}" +
         " loading=\"lazy\" decoding=\"async\" />";
   }

   private static string ParagraphizeHtml(string text)
   {
      var encoded = WebUtility.HtmlEncode(text);

      var paragraphs = encoded.Split(
         new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

      if (paragraphs.Length == 0)
      {
         return string.Empty;
      }

      return string.Join(
         string.Empty,
         Array.ConvertAll(paragraphs, paragraph => $"<p>{paragraph}</p>"));
   }

   private static string GetAssetText(StoredAsset content)
   {
      if (content.Text is null)
      {
         throw new InvalidOperationException(
            $"{content.AssetKey} does not contain text.");
      }

      return content.Text;
   }

   private static string GetAssetUrl(string assetKey)
   {
      return $"/content/{assetKey}";
   }

   private const string _htmlContentType = "text/html";
   private const string _plainTextContentType = "text/plain";
   private const string _markdownContentType = "text/markdown";
   private const string _imageContentTypePrefix = "image/";

   private readonly ILogger<BasicContentRenderer> _logger;
}