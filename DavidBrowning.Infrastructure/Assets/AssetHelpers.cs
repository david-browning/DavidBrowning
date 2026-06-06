// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Security.Cryptography;
using System.Text;
using DavidBrowning.Helpers;

namespace DavidBrowning.Infrastructure.Assets;

internal static class AssetHelpers
{
   public static string GetContentType(string assetKey)
   {
      var extension = Path.GetExtension(assetKey);

      return extension.ToLowerInvariant() switch
      {
         ".md" => "text/markdown",
         ".markdown" => "text/markdown",
         ".html" => "text/html",
         ".txt" => "text/plain",
         ".json" => "application/json",
         ".jpg" => "image/jpeg",
         ".jpeg" => "image/jpeg",
         ".png" => "image/png",
         ".webp" => "image/webp",
         ".gif" => "image/gif",
         ".css" => "text/css",
         ".js" => "text/javascript",
         ".pdf" => "application/pdf",
         ".svg" => "image/svg+xml",
         _ => "application/octet-stream",
      };
   }

   public static string GetEntityTag(
      string assetKey,
      DateTimeOffset lastModifiedUtc,
      long? contentLength)
   {
      var source = string.Join(
         "|",
         assetKey.ToLowerInvariant(),
         lastModifiedUtc.UtcTicks,
         contentLength?.ToString() ?? string.Empty);

      var sourceBytes = Encoding.UTF8.GetBytes(source);
      var hashBytes = SHA256.HashData(sourceBytes);
      var hash = Convert.ToHexString(hashBytes).ToLowerInvariant();

      return $"\"{hash}\"";
   }

   public static bool IsTextContentType(string contentType)
   {
      var mediaType = GetMediaType(contentType);

      return mediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase) || IsJsonContentType(mediaType);
   }

   public static bool IsJsonContentType(string contentType)
   {
      var mediaType = GetMediaType(contentType);

      return mediaType.EqualsOrdinalIgnoreCase("application/json") ||
         mediaType.EndsWith("+json", StringComparison.OrdinalIgnoreCase);
   }

   public static string GetMediaType(string contentType)
   {
      var separatorIndex = contentType.IndexOf(';');

      return separatorIndex < 0
         ? contentType.Trim()
         : contentType[..separatorIndex].Trim();
   }
}