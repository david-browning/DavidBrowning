// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DavidBrowning.Services.Assets
{
   public sealed class LocalContentService : IContentService
   {
      public LocalContentService(
         IConfiguration configuration,
         IHostEnvironment hostEnvironment)
      {
         var contentRoot = configuration["Stores:ContentStore:LocalRoot"];
         if (string.IsNullOrEmpty(contentRoot))
         {
            throw new InvalidOperationException(
               "The content store local root is not set.");
         }

         _contentRoot = Path.GetFullPath(Path.Combine(
            hostEnvironment.ContentRootPath, contentRoot));
      }

      public async Task<StoredContent> GetContentAsync(
         string assetKey,
         CancellationToken cancellationToken = default)
      {
         var fullPath = GetAssetFullPath(assetKey);
         if (!File.Exists(fullPath))
         {
            throw new FileNotFoundException(
               "File asset bit found. Asset keys are case-sensitive.", 
               fullPath);
         }

         var contentType = GetSourceFormat(fullPath);
         var fileText = await File.ReadAllTextAsync(fullPath, cancellationToken);

         return new StoredContent()
         {
            AssetKey = assetKey,
            SourceFormat = contentType,
            SourceText = fileText,
         };
      }

      private string GetAssetFullPath(string assetKey)
      {
         if (string.IsNullOrWhiteSpace(assetKey))
         {
            throw new ArgumentException(
               "Asset key cannot be null, empty, or whitespace.",
               nameof(assetKey));
         }

         if (assetKey.StartsWith('/') || assetKey.StartsWith('\\'))
         {
            throw new ArgumentException("Asset keys cannot begin with a slash");
         }

         if(Path.IsPathRooted(assetKey))
         {
            throw new ArgumentException(
               "Asset keys must not be rooted file paths.",
               nameof(assetKey));
         }

         var normalizedAssetKey = assetKey
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);

         var fullPath = Path.GetFullPath(
            Path.Combine(_contentRoot, normalizedAssetKey));

         return fullPath;
      }

      private static ContentSourceFormat GetSourceFormat(string fullPath)
      {
         var extension = Path.GetExtension(fullPath);
         return extension.ToLowerInvariant() switch
         {
            ".md" => ContentSourceFormat.Markdown,
            ".markdown" => ContentSourceFormat.Markdown,
            ".html" => ContentSourceFormat.Html,
            ".txt" => ContentSourceFormat.PlainText,
            _ => ContentSourceFormat.Unknown,
         };
      }

      private readonly string _contentRoot;
   }
}
