// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Models.ViewModels;

public sealed class AssetBlockViewModel
{
   public AssetBlockViewModel()
   {

   }

   [SetsRequiredMembers]
   public AssetBlockViewModel(ProjectAssetLink projectAsset)
   {
      AssetType = projectAsset.SiteAsset!.AssetType;
      AltText = !string.IsNullOrEmpty(projectAsset.AltTextOverride) ?
         projectAsset.AltTextOverride : projectAsset.SiteAsset!.AltText;
   }

   [SetsRequiredMembers]
   public AssetBlockViewModel(SiteAssetLink siteAsset)
   {
      AssetType = siteAsset.SiteAsset!.AssetType;
      Caption = siteAsset.SiteAsset!.AltText;
      AltText = siteAsset.SiteAsset!.AltText;
   }

   [SetsRequiredMembers]
   public AssetBlockViewModel(SiteAsset asset)
   {
      AltText = asset.AltText;
   }

   public required SiteAssetType AssetType { get; set; }

   public string? Caption { get; set; }

   public string? AltText { get; set; }

   public string? Description { get; set; }

   public string? Slug { get; set; }
}
