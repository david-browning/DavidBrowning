using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using DavidBrowning.Infrastructure.Assets;
using DavidBrowning.Models.Published;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Infrastructure.Publishing;

/// <summary>
/// Compiles the site snapshot and publishes the content to the injected 
/// IContentStore. This could be a local file system or an Azure storage blob.
/// </summary>
public sealed class PublicSitePublisher : IPublicSitePublisher
{
   public PublicSitePublisher(
      IPublicSiteSnapshotBuilder snapshotBuilder,
      IOptions<PublicSitePublicationOptions> options,
      IContentStore contentStore,
      TimeProvider timeProvider)
   {
      _snapshotBuilder = snapshotBuilder;
      _options = options.Value;
      _contentStore = contentStore;
      _timeProvider = timeProvider;
   }

   public async Task<PublishResult> PublishAsync(
      CancellationToken cancellationToken = default)
   {
      // Build the snapshot of the website database content:
      DateTimeOffset publishedAtUtc = _timeProvider.GetUtcNow();
      string version = publishedAtUtc.ToString(
         "yyyyMMdd'T'HHmmssfff'Z'", CultureInfo.InvariantCulture);
      string snapshotKey =
         $"{_options.SnapshotPrefix.TrimEnd('/')}/{version}/site.json";
      var snapshot = await _snapshotBuilder.BuildAsync(
         version, publishedAtUtc, cancellationToken);

      // Write an immutable snapshot to the blob storage. This operation must 
      // complete before updating the "current" snapshot.
      var writeResult = await WriteToStoreAsync(
         snapshotKey, snapshot, cancellationToken);
      PublishedSiteManifest manifest = new()
      {
         Version = version,
         SnapshotKey = snapshotKey,
         PublishedAtUtc = publishedAtUtc,
      };

      var commitPointResult = await WriteToStoreAsync(
         _options.ManifestKey, manifest, cancellationToken);
      return new PublishResult()
      {
         Version = version,
         SnapshotKey = snapshotKey,
         PublishedAtUtc = publishedAtUtc,
         SnapshotWriteResult = writeResult,
         ManifestWriteResult = commitPointResult,
      };
   }

   private async Task<ContentWriteResults> WriteToStoreAsync<T>(
      string assetKey,
      T value,
      CancellationToken cancellationToken = default)
      where T : notnull
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(assetKey);
      ArgumentNullException.ThrowIfNull(value);

      await using MemoryStream stream = new();
      await JsonSerializer.SerializeAsync(
         stream, value, _serializerOptions, cancellationToken);
      stream.Position = 0;
      return await _contentStore.WriteAsync(
         assetKey, stream, cancellationToken);
   }

   private static readonly JsonSerializerOptions _serializerOptions = new()
   {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
      WriteIndented = false,
      Converters =
      {
         new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
      },
   };

   private readonly IPublicSiteSnapshotBuilder _snapshotBuilder;
   private readonly PublicSitePublicationOptions _options;
   private readonly TimeProvider _timeProvider;
   private readonly IContentStore _contentStore;
}
