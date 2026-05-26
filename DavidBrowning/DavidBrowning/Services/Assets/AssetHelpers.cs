// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DavidBrowning.Models.ViewModels;

namespace DavidBrowning.Services.Assets;

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

   public static bool IsTextSourceFormat(ContentSourceFormat sourceFormat)
   {
      return sourceFormat == ContentSourceFormat.Markdown ||
         sourceFormat == ContentSourceFormat.Html ||
         sourceFormat == ContentSourceFormat.PlainText ||
         sourceFormat == ContentSourceFormat.Json;
   }

   public static ContentSourceFormat GetSourceFormat(string fullPath)
   {
      var extension = Path.GetExtension(fullPath);
      return extension.ToLowerInvariant() switch
      {
         ".md" => ContentSourceFormat.Markdown,
         ".markdown" => ContentSourceFormat.Markdown,
         ".html" => ContentSourceFormat.Html,
         ".txt" => ContentSourceFormat.PlainText,
         ".json" => ContentSourceFormat.Json,
         ".jpg" => ContentSourceFormat.Image,
         ".jpeg" => ContentSourceFormat.Image,
         ".png" => ContentSourceFormat.Image,
         ".webp" => ContentSourceFormat.Image,
         ".gif" => ContentSourceFormat.Image,
         _ => ContentSourceFormat.Unknown,
      };
   }
}
