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

   public async Task<ContentWriteResults> WriteAsync(
      string assetKey,
      Stream contentStream,
      CancellationToken cancellationToken = default)
   {
      cancellationToken.ThrowIfCancellationRequested();
      var fullPath = GetAssetFullPath(assetKey);

      if (File.Exists(fullPath))
      {
         await WriteContentAsync(contentStream, fullPath, cancellationToken);
         return ContentWriteResults.Overwritten;
      }

      await WriteContentAsync(contentStream, fullPath, cancellationToken);
      return ContentWriteResults.CreatedNew;
   }

   private static async Task WriteContentAsync(
      Stream contentStream,
      string fullPath,
      CancellationToken cancellationToken)
   {
      ArgumentNullException.ThrowIfNull(contentStream);
      ArgumentException.ThrowIfNullOrWhiteSpace(fullPath);
      string? directoryPath = Path.GetDirectoryName(fullPath);
      if (string.IsNullOrWhiteSpace(directoryPath))
      {
         throw new InvalidOperationException(
            $"Could not determine the parent directory for '{fullPath}'.");
      }

      Directory.CreateDirectory(directoryPath);

      string temporaryPath = Path.Combine(
         directoryPath, $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");

      try
      {
         await using (var destinationStream = new FileStream(
            temporaryPath, new FileStreamOptions()
            {
               Access = FileAccess.Write,
               Mode = FileMode.CreateNew,
               Share = FileShare.None,
               Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
            }))
         {
            await contentStream.CopyToAsync(
               destinationStream, cancellationToken);

            await destinationStream.FlushAsync(cancellationToken);
         }

         File.Move(temporaryPath, fullPath, overwrite: true);
      }
      catch
      {
         TryDeleteFile(temporaryPath);
         throw;
      }
   }

   public Task DeleteFileAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(assetKey);
      cancellationToken.ThrowIfCancellationRequested();
      var fullPath = GetAssetFullPath(assetKey);
      File.Delete(fullPath);
      return Task.CompletedTask;
   }

   private static void TryDeleteFile(string fullPath)
   {
      try
      {
         File.Delete(fullPath);
      }
      catch
      {
         // Preserve the original write failure.
         // A stale temporary file is less important than the root cause.
      }
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

      if (assetKey.Contains("..", StringComparison.Ordinal))
      {
         throw new InvalidOperationException(
            $"Asset key contains an invalid path segment: '{assetKey}'.");
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
            "Asset key escapes the configured content root.", nameof(assetKey));
      }

      return fullPath;
   }

   private readonly string _contentRoot;
}