// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DavidBrowning.Infrastructure.Assets;

/// <summary>
/// Content store that gets content from the local machine.
/// </summary>
public sealed class LocalContentStore : IContentStore
{
   public LocalContentStore(
      IConfiguration configuration,
      IHostEnvironment hostEnvironment)
   {
      var contentRoot = configuration["Stores:ContentStore:LocalRoot"];

      if (string.IsNullOrWhiteSpace(contentRoot))
      {
         throw new InvalidOperationException(
            "The content store local root is not set.");
      }

      _contentRoot = Path.GetFullPath(
         Path.Combine(hostEnvironment.ContentRootPath, contentRoot));
   }

   public async Task<StoredAsset> GetAssetAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      var fullPath = GetAssetFullPath(assetKey);
      if (!File.Exists(fullPath))
      {
         throw new FileNotFoundException(
            "File asset not found. Asset keys are case-sensitive.",
            fullPath);
      }

      var fileInfo = new FileInfo(fullPath);
      var contentType = AssetHelpers.GetContentType(assetKey);

      string? text = null;
      if (AssetHelpers.IsTextContentType(contentType))
      {
         text = await File.ReadAllTextAsync(fullPath, cancellationToken);
      }

      var lastModifiedUtc = new DateTimeOffset(fileInfo.LastWriteTimeUtc);

      return new StoredAsset()
      {
         AssetKey = assetKey,
         ContentType = contentType,
         Text = text,
         ContentLength = fileInfo.Length,
         EntityTag = AssetHelpers.GetEntityTag(
            assetKey, lastModifiedUtc, fileInfo.Length),
         LastModifiedUtc = lastModifiedUtc,
      };
   }

   public Task<Stream> OpenReadAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      cancellationToken.ThrowIfCancellationRequested();

      var fullPath = GetAssetFullPath(assetKey);

      if (!File.Exists(fullPath))
      {
         throw new FileNotFoundException(
            "File asset not found. Asset keys are case-sensitive.",
            fullPath);
      }

      Stream stream = File.OpenRead(fullPath);

      return Task.FromResult(stream);
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
         throw new ArgumentException(
            "Asset keys cannot begin with a slash.",
            nameof(assetKey));
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

      var contentRoot = _contentRoot.TrimEnd(
         Path.DirectorySeparatorChar,
         Path.AltDirectorySeparatorChar);

      contentRoot += Path.DirectorySeparatorChar;

      var comparison = OperatingSystem.IsWindows()
         ? StringComparison.OrdinalIgnoreCase
         : StringComparison.Ordinal;

      if (!fullPath.StartsWith(contentRoot, comparison))
      {
         throw new ArgumentException(
            "Asset key escapes the configured content root.",
            nameof(assetKey));
      }

      return fullPath;
   }

   private readonly string _contentRoot;
}