// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Infrastructure.Assets;

namespace DavidBrowning.Admin.ViewModels.Asset;

public sealed class ContentTypePickerViewModel
{
   public const string DefaultAssetKey =
      "Documents/content-types.json";

   public required IReadOnlyList<ContentTypeOptionViewModel> Options
   {
      get;
      init;
   }

   public string? SelectedContentType { get; init; }

   public static async Task<ContentTypePickerViewModel> LoadAsync(
      IContentStore contentStore,
      string? selectedContentType = null,
      string assetKey = DefaultAssetKey,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(contentStore);

      StoredAsset asset = await contentStore.GetAssetAsync(
         assetKey,
         cancellationToken);

      if (string.IsNullOrWhiteSpace(asset.Text))
      {
         throw new InvalidOperationException(
            $"Content-type asset '{assetKey}' does not contain " +
            "readable JSON text.");
      }

      ContentTypeOptionViewModel[]? configuredOptions =
         JsonSerializer.Deserialize<ContentTypeOptionViewModel[]>(
            asset.Text,
            new JsonSerializerOptions
            {
               PropertyNameCaseInsensitive = true,
            });

      if (configuredOptions is null)
      {
         throw new InvalidOperationException(
            $"Content-type asset '{assetKey}' did not contain " +
            "a JSON array.");
      }

      ContentTypeOptionViewModel[] options = configuredOptions
         .Select(option => Normalize(option, assetKey))
         .DistinctBy(
            option => option.ContentType,
            StringComparer.OrdinalIgnoreCase)
         .OrderBy(option => option.Category, StringComparer.Ordinal)
         .ThenBy(option => option.DisplayName, StringComparer.Ordinal)
         .ToArray();

      return new ContentTypePickerViewModel()
      {
         Options = options,
         SelectedContentType = selectedContentType,
      };
   }

   public bool Supports(string? contentType)
   {
      if (string.IsNullOrWhiteSpace(contentType))
      {
         return false;
      }

      return Options.Any(
         option => string.Equals(
            option.ContentType,
            contentType,
            StringComparison.OrdinalIgnoreCase));
   }

   private static ContentTypeOptionViewModel Normalize(
      ContentTypeOptionViewModel option,
      string assetKey)
   {
      string contentType = option.ContentType.Trim();
      string displayName = option.DisplayName.Trim();
      string category = option.Category.Trim();

      if (string.IsNullOrWhiteSpace(contentType) ||
          string.IsNullOrWhiteSpace(displayName) ||
          string.IsNullOrWhiteSpace(category))
      {
         throw new InvalidOperationException(
            $"Content-type asset '{assetKey}' contains an incomplete " +
            "entry.");
      }

      if (!contentType.Contains('/'))
      {
         throw new InvalidOperationException(
            $"Content-type asset '{assetKey}' contains an invalid " +
            $"MIME type: '{contentType}'.");
      }

      return new ContentTypeOptionViewModel()
      {
         ContentType = contentType,
         DisplayName = displayName,
         Category = category,
      };
   }
}