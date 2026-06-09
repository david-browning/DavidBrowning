using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels.About;
using DavidBrowning.Infrastructure.Assets;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DavidBrowning.Admin.ViewModels;

public sealed partial class FontAwesomeIconPickerViewModel
{
   public const string DefaultAssetKey =
      "Documents/fontawesome-icons.json";

   public required IReadOnlyList<string> SupportedIconCssClasses
   {
      get;
      init;
   }

   public string? SelectedIconCssClass { get; init; }

   public static async Task<FontAwesomeIconPickerViewModel> LoadAsync(
      IContentStore contentStore,
      string? selectedIconCssClass = null,
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

      string[]? configuredIcons =
         JsonSerializer.Deserialize<string[]>(asset.Text);

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
         if (!IconCssPattern().IsMatch(icon))
         {
            throw new InvalidOperationException(
               $"Font Awesome icon asset '{assetKey}' contains an " +
               $"invalid CSS class value: '{icon}'.");
         }
      }

      return new FontAwesomeIconPickerViewModel()
      {
         SelectedIconCssClass = selectedIconCssClass,
         SupportedIconCssClasses = supportedIcons,
      };
   }

   public bool Supports(string? iconCssClass)
   {
      if (string.IsNullOrWhiteSpace(iconCssClass))
      {
         return true;
      }

      return SupportedIconCssClasses.Contains(
         iconCssClass,
         StringComparer.Ordinal);
   }


   public static async Task<FontAwesomeIconPickerViewModel> LoadAndValidateIconPickerAsync(
      IContentStore contentStore,
      InterestEditViewModel model,
      ModelStateDictionary modelState,
      CancellationToken cancellationToken)
   {
      FontAwesomeIconPickerViewModel iconPicker =
         await LoadIconPickerAsync(
            contentStore, model.SelectedIconCssClass, cancellationToken);
      if (!iconPicker.Supports(model.SelectedIconCssClass))
      {
         modelState.AddModelError(nameof(model.SelectedIconCssClass), "Select a supported icon.");
      }

      model.IconPicker = iconPicker;
      return iconPicker;
   }

   public static Task<FontAwesomeIconPickerViewModel> LoadIconPickerAsync(
      IContentStore contentStore,
      string? selectedIconCssClass,
      CancellationToken cancellationToken)
   {
      return LoadAsync(
         contentStore, selectedIconCssClass, cancellationToken: cancellationToken);
   }


   [GeneratedRegex(
      @"^fa-(solid|regular|brands)(?:\s+fa-[a-z0-9-]+)+$",
      RegexOptions.CultureInvariant)]
   private static partial Regex IconCssPattern();
}