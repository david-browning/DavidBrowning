// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Services.Assets;

namespace DavidBrowning.Services.Cache;

public sealed class JsonCache
{
   public JsonCache(IContentStore contentStore, JsonMemoryCache cache)
   {
      _contentStore = contentStore;
      _cache = cache;
   }

   public async Task<T> GetJsonFileContentAsync<T>(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      var cacheKey = GetJsonCacheKey<T>(assetKey);

      var cachedValue = await _cache.GetOrCreateAsync(
         cacheKey,
         token => DeserializeAsync<T>(assetKey, token),
         cancellationToken);

      if (cachedValue is T typedValue)
      {
         return typedValue;
      }

      throw new InvalidOperationException(
         $"Cached JSON content for '{assetKey}' was not of type " +
         $"'{typeof(T).FullName}'. Actual type was " +
         $"'{cachedValue?.GetType().FullName ?? "null"}'.");
   }

   private async Task<object> DeserializeAsync<T>(
      string assetKey,
      CancellationToken cancellationToken)
   {
      var asset = await _contentStore.GetAssetAsync(
         assetKey, cancellationToken);

      if (!AssetHelpers.IsJsonContentType(asset.ContentType))
      {
         throw new InvalidOperationException(
            $"{assetKey} is not a JSON file.");
      }

      if (string.IsNullOrWhiteSpace(asset.Text))
      {
         throw new InvalidOperationException(
            $"JSON asset {assetKey} does not contain any text.");
      }

      var model = JsonSerializer.Deserialize<T>(
         asset.Text,
         new JsonSerializerOptions()
         {
            PropertyNameCaseInsensitive = true,
         });

      if (model is null)
      {
         throw new InvalidOperationException(
            $"JSON asset {assetKey} could not be parsed as " +
            $"{typeof(T).Name}.");
      }

      return model;
   }

   private static string GetJsonCacheKey<T>(string assetKey)
   {
      return $"json-content:{typeof(T).AssemblyQualifiedName}:{assetKey}";
   }

   private readonly IContentStore _contentStore;
   private readonly JsonMemoryCache _cache;
}