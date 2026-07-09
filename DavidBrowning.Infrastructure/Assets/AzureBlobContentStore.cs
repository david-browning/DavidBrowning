// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using Azure;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DavidBrowning.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Infrastructure.Assets;

public sealed class AzureBlobContentStore : IContentStore
{
   public AzureBlobContentStore(
      IOptions<AzureBlobContentStoreOptions> options,
      TokenCredential tokenCredential)
   {
      ArgumentNullException.ThrowIfNull(options);
      ArgumentNullException.ThrowIfNull(tokenCredential);

      AzureBlobContentStoreOptions storeOptions = options.Value;

      ArgumentException.ThrowIfNullOrWhiteSpace(storeOptions.ServiceUri);
      ArgumentException.ThrowIfNullOrWhiteSpace(storeOptions.ContainerName);

      BlobServiceClient serviceClient = new(
         new Uri(storeOptions.ServiceUri), tokenCredential);

      _containerClient = serviceClient.GetBlobContainerClient(
         storeOptions.ContainerName);
   }

   public async Task<StoredAsset> GetAssetAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      ValidateAssetKey(assetKey);
      BlobClient blobClient = _containerClient.GetBlobClient(assetKey);

      try
      {
         BlobProperties properties = await blobClient.GetPropertiesAsync(
            cancellationToken: cancellationToken);

         string contentType = ResolveContentType(assetKey, properties.ContentType);

         string? text = null;
         if (AssetHelpers.IsTextContentType(contentType))
         {
            BlobDownloadResult download = await blobClient.DownloadContentAsync(
               cancellationToken);
            text = download.Content.ToString();
         }

         return new StoredAsset()
         {
            AssetKey = assetKey,
            ContentType = contentType,
            ContentLength = properties.ContentLength,
            Text = text,
            EntityTag = properties.ETag.ToString(),
            LastModifiedUtc = properties.LastModified,
         };
      }
      catch (RequestFailedException ex) when (ex.Status == 404)
      {
         throw new FileNotFoundException(
            "Blob asset not found. Asset keys are case-sensitive.",
            assetKey, ex);
      }
   }

   public async Task<bool> AssetExistsAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      ValidateAssetKey(assetKey);
      BlobClient blobClient = _containerClient.GetBlobClient(assetKey);
      Response<bool> exists = await blobClient.ExistsAsync(cancellationToken);
      return exists.Value;
   }

   public async Task<Stream> OpenReadAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      ValidateAssetKey(assetKey);
      BlobClient blobClient = _containerClient.GetBlobClient(assetKey);

      try
      {
         Response<BlobDownloadStreamingResult> download =
            await blobClient.DownloadStreamingAsync(
               cancellationToken: cancellationToken);
         return download.Value.Content;
      }
      catch (RequestFailedException ex) when (ex.Status == 404)
      {
         throw new FileNotFoundException(
            "Blob asset not found. Asset keys are case-sensitive.",
            assetKey, ex);
      }
   }

   public async Task<ContentWriteResults> WriteAsync(
      string assetKey,
      Stream contentStream,
      CancellationToken cancellationToken = default)
   {
      ValidateAssetKey(assetKey);
      ArgumentNullException.ThrowIfNull(contentStream);

      await _containerClient.CreateIfNotExistsAsync(
         cancellationToken: cancellationToken);
      BlobClient blobClient = _containerClient.GetBlobClient(assetKey);
      bool existed = await blobClient.ExistsAsync(cancellationToken);
      if (contentStream.CanSeek)
      {
         contentStream.Position = 0;
      }

      var uploadOptions = new BlobUploadOptions()
      {
         HttpHeaders = new BlobHttpHeaders()
         {
            ContentType = AssetHelpers.GetContentType(assetKey),
         },
      };

      await blobClient.UploadAsync(
         contentStream, uploadOptions, cancellationToken);
      return existed ?
         ContentWriteResults.Overwritten : ContentWriteResults.CreatedNew;
   }

   public async Task DeleteFileAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      ValidateAssetKey(assetKey);
      BlobClient blobClient = _containerClient.GetBlobClient(assetKey);
      await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
   }

   private static void ValidateAssetKey(string assetKey)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(assetKey);

      if (assetKey.StartsWith('/') || assetKey.StartsWith('\\'))
      {
         throw new ArgumentException(
            "Asset keys cannot begin with a slash.", nameof(assetKey));
      }

      if (assetKey.Contains("..", StringComparison.Ordinal))
      {
         throw new InvalidOperationException(
            $"Asset key contains an invalid path segment: '{assetKey}'.");
      }

      if (Path.IsPathRooted(assetKey))
      {
         throw new ArgumentException(
            "Asset keys must not be rooted file paths.", nameof(assetKey));
      }
   }

   private static string ResolveContentType(
      string assetKey,
      string? blobContentType)
   {
      string inferredContentType = AssetHelpers.GetContentType(assetKey);

      if (!IsFallbackContentType(inferredContentType))
      {
         return inferredContentType;
      }

      if (!string.IsNullOrWhiteSpace(blobContentType))
      {
         return blobContentType;
      }

      return inferredContentType;
   }

   private static bool IsFallbackContentType(string contentType)
   {
      return contentType.Equals(
         "application/octet-stream",
         StringComparison.OrdinalIgnoreCase);
   }

   private readonly BlobContainerClient _containerClient;
}