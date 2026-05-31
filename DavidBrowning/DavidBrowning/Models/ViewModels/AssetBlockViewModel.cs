// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Models.ViewModels;

public sealed class AssetBlockViewModel
{
   public AssetBlockViewModel()
   {
   }

   [SetsRequiredMembers]
   public AssetBlockViewModel(ProjectAssetLink link)
   {
      var asset = link.SiteAsset ??
         throw new InvalidOperationException(
            "Project asset link is missing its site asset.");

      AssetKey = asset.AssetKey;
      AssetType = asset.AssetType;
      AltText = link.AltTextOverride ?? asset.AltText;
      Caption = link.Caption;
      RoleSlug = link.ProjectAssetRole?.Slug;
      RoleDisplayName = link.ProjectAssetRole?.DisplayName;
      SortOrder = link.SortOrder;
      WidthPixels = asset.WidthPixels;
      HeightPixels = asset.HeightPixels;
   }

   [SetsRequiredMembers]
   public AssetBlockViewModel(SiteAssetLink link)
   {
      var asset = link.SiteAsset ??
         throw new InvalidOperationException(
            "Post asset link is missing its site asset.");

      AssetKey = asset.AssetKey;
      AssetType = asset.AssetType;
      AltText = link.AltTextOverride ?? asset.AltText;
      Caption = link.Caption;
      RoleSlug = link.Role.ToString();
      RoleDisplayName = link.Role.ToString();
      SortOrder = link.SortOrder;
      WidthPixels = asset.WidthPixels;
      HeightPixels = asset.HeightPixels;
   }

   public required string AssetKey { get; init; }

   public required SiteAssetType AssetType { get; init; }

   public string? RoleSlug { get; init; }

   public string? RoleDisplayName { get; init; }

   public string? Caption { get; init; }

   public string? AltText { get; init; }

   public int SortOrder { get; init; }

   public int? WidthPixels { get; init; }

   public int? HeightPixels { get; init; }
}