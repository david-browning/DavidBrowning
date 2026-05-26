// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DavidBrowning.Services.Assets;

/// <summary>
/// A content store that gets content from the local machine.
/// The root content folder is stored in the configuration file under
/// Stores:ContentStore:LocalRoot
/// </summary>
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

   public async Task<StoredAsset> GetAssetAsync(
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

      var contentType = AssetHelpers.GetSourceFormat(fullPath);
      string fileText = string.Empty;
      if (AssetHelpers.IsTextSourceFormat(contentType))
      {
         fileText = await File.ReadAllTextAsync(fullPath, cancellationToken);
      }

      var fileInfo = new FileInfo(fullPath);
      var fileModified = fileInfo.LastWriteTimeUtc;
      var fileSize = fileInfo.Length;
      var entityTag = AssetHelpers.GetEntityTag(
         assetKey, fileModified, fileSize);

      return new StoredAsset()
      {
         AssetKey = assetKey,
         SourceFormat = contentType,
         Text = fileText,
         ContentLength = fileSize,
         EntityTag = entityTag,
         LastModifiedUtc = fileModified,
      };
   }

   public Task<Stream> OpenReadAsync(
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

      Stream stream = File.OpenRead(fullPath);
      return Task.FromResult(stream);
   }

   public string GetAssetFileType(string assetKey)
   {
      return AssetHelpers.GetContentType(assetKey);
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

      if (Path.IsPathRooted(assetKey))
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

   private readonly string _contentRoot;
}
