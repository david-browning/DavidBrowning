// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Web.ViewModels;

public sealed class AssetBlockViewModel
{
   [SetsRequiredMembers]
   public AssetBlockViewModel(ProjectAssetLink link)
   {
      var asset = link.SiteAsset ??
         throw new InvalidOperationException(
            "Project asset link is missing its site asset.");

      var role = link.ProjectAssetRole ??
         throw new InvalidOperationException(
            "Project asset link is missing its role.");

      AssetKey = asset.AssetKey;
      ContentType = asset.ContentType;
      AltText = link.AltTextOverride ?? asset.AltText;
      Caption = link.Caption;
      WidthPixels = asset.WidthPixels;
      HeightPixels = asset.HeightPixels;
   }

   [SetsRequiredMembers]
   public AssetBlockViewModel(PostRevisionAssetLink link)
   {
      var asset = link.SiteAsset ??
         throw new InvalidOperationException(
            "Post asset link is missing its site asset.");

      AssetKey = asset.AssetKey;
      ContentType = asset.ContentType;
      AltText = link.AltTextOverride ?? asset.AltText;
      Caption = link.Caption;
      WidthPixels = asset.WidthPixels;
      HeightPixels = asset.HeightPixels;
   }

   public required string AssetKey { get; init; }

   public required string ContentType { get; init; }

   public string? Caption { get; init; }

   public string? AltText { get; init; }

   public int? WidthPixels { get; init; }

   public int? HeightPixels { get; init; }
}