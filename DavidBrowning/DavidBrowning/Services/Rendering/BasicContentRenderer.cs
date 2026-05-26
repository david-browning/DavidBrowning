using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Services.Assets;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Services.Rendering
{
   public sealed class BasicContentRenderer : IContentRenderer
   {
      public BasicContentRenderer(
         ILogger<BasicContentRenderer> logger)
      {
         _logger = logger;
      }

      public Task<RenderedContent> RenderAsync(
         StoredAsset content,
         ContentRenderOptions? options = null,
         CancellationToken cancellationToken = default)
      {
         string html = string.Empty;
         switch (content.SourceFormat)
         {
            case ContentSourceFormat.Html:
            {
               html = GetHtml(content, options);
               break;
            }
            case ContentSourceFormat.PlainText:
            {
               html = GetTextHtml(content, options);
               break;
            }
            case ContentSourceFormat.Markdown:
            {
               html = GetMarkdownHtml(content, options);
               break;
            }
            case ContentSourceFormat.Image:
            {
               html = GetImageHtml(content, options);
               break;
            }
            default:
            {
               throw new InvalidOperationException(
                  $"Unsupported asset source format: {content.SourceFormat}.");
            }
         }

         var ret = new RenderedContent()
         {
            AssetKey = content.AssetKey,
            OriginalSourceFormat = content.SourceFormat,
            Html = html,
         };

         return Task.FromResult(ret);
      }

      private string GetHtml(
         StoredAsset content,
         ContentRenderOptions? options)
      {
         // TODO: Sanitize the HTML
         throw new InvalidOperationException(
            "Raw HTML content is not supported yet.");
      }

      private string GetMarkdownHtml(
         StoredAsset content,
         ContentRenderOptions? options)
      {
         // TODO: Sanitize the HTML
         throw new InvalidOperationException(
            "Markdown content is not supported yet.");
      }

      private string GetTextHtml(
         StoredAsset content,
         ContentRenderOptions? options)
      {
         return ParagaphitizeHtml(GetAssetText(content));
      }

      private string GetImageHtml(
         StoredAsset content,
         ContentRenderOptions? options)
      {
         if (string.IsNullOrEmpty(options?.AltText))
         {
            _logger.LogWarning(
               "Image assets should have the alt text option set.");
         }

         var src = WebUtility.HtmlEncode(GetAssetUrl(content.AssetKey));
         var altText = WebUtility.HtmlEncode(options?.AltText);

         var cssAttr = string.Empty;
         if (options != null && !string.IsNullOrEmpty(options.CssClass))
         {
            cssAttr = WebUtility.HtmlEncode(options!.CssClass);
         }

         return
            $"<img src=\"{src}\" alt=\"{altText}\"" +
            $"class=\"{cssAttr}\" loading=\"lazy\" decoding=\"async\" />";
      }

      private string ParagaphitizeHtml(string text)
      {
         // Encode the text
         var encoded = WebUtility.HtmlEncode(text);

         // Split by newline
         var paragraphs = encoded.Split(
            new[] { "\r\n\r\n", "\n\n" },
            StringSplitOptions.RemoveEmptyEntries);

         if (paragraphs.Length == 0)
         {
            return string.Empty;
         }

         // Put all the sections back together where each one is enclosed in a 
         // paragraph tag.
         return string.Join(
            string.Empty,
            Array.ConvertAll(paragraphs, paragraph => $"<p>{paragraph}</p>"));
      }

      private string GetAssetText(StoredAsset content)
      {
         if (content.Text == null)
         {
            throw new InvalidOperationException(
               $"{content.AssetKey} does not contain text.");
         }

         return content.Text;
      }

      private string GetAssetUrl(string assetKey)
      {
         return $"content/{assetKey}";
      }

      private readonly ILogger<BasicContentRenderer> _logger;
   }
}
