using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Infrastructure.Assets;

namespace DavidBrowning.Admin.ViewModels;

public sealed partial class FontAwesomeIconPickerViewModel
{
   public const string DefaultAssetKey = "Documents/fontawesome-icons.json";

   public required IReadOnlyList<string> SupportedIconCSSClasses { get; init; }

   public string? SelectedIconCSS { get; init; }

   public static async Task<FontAwesomeIconPickerViewModel> LoadAsync(
      IContentStore contentStore,
      string? selectedIconCSS = null,
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
            $"Font Awesome icon asset '{assetKey}' does not contain " +
            "readable JSON text.");
      }

      string[]? configuredIcons = JsonSerializer.Deserialize<string[]>(
         asset.Text);

      if (configuredIcons is null)
      {
         throw new InvalidOperationException(
            $"Font Awesome icon asset '{assetKey}' did not contain " +
            "a JSON array.");
      }

      string[] supportedIcons = configuredIcons
         .Select(icon => icon.Trim())
         .Where(icon => !string.IsNullOrWhiteSpace(icon))
         .Distinct(StringComparer.Ordinal)
         .OrderBy(icon => icon, StringComparer.Ordinal)
         .ToArray();

      foreach (string icon in supportedIcons)
      {
         if (!IconCSSPattern().IsMatch(icon))
         {
            throw new InvalidOperationException(
               $"Font Awesome icon asset '{assetKey}' contains an " +
               $"invalid CSS class value: '{icon}'.");
         }
      }

      return new FontAwesomeIconPickerViewModel()
      {
         SelectedIconCSS = selectedIconCSS,
         SupportedIconCSSClasses = supportedIcons,
      };
   }

   public bool Supports(string? iconCSS)
   {
      if (string.IsNullOrWhiteSpace(iconCSS))
      {
         return true;
      }

      return SupportedIconCSSClasses.Contains(
         iconCSS,
         StringComparer.Ordinal);
   }

   [GeneratedRegex(
      @"^fa-(solid|regular|brands)(?:\s+fa-[a-z0-9-]+)+$",
      RegexOptions.CultureInvariant)]
   private static partial Regex IconCSSPattern();
}