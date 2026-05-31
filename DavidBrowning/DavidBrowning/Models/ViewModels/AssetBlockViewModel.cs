// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Models.ViewModels;

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
      OriginalFileName = asset.OriginalFileName;
      AltText = link.AltTextOverride ?? asset.AltText;
      Caption = link.Caption;
      RoleSlug = role.Slug;
      RoleDisplayName = role.DisplayName;
      SortOrder = link.SortOrder;
      SizeBytes = asset.SizeBytes;
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
      ContentType = asset.ContentType;
      OriginalFileName = asset.OriginalFileName;
      AltText = link.AltTextOverride ?? asset.AltText;
      Caption = link.Caption;
      RoleSlug = GetRoleSlug(link.Role);
      RoleDisplayName = GetRoleDisplayName(link.Role);
      SortOrder = link.SortOrder;
      SizeBytes = asset.SizeBytes;
      WidthPixels = asset.WidthPixels;
      HeightPixels = asset.HeightPixels;
   }

   public required string AssetKey { get; init; }

   public required string ContentType { get; init; }

   public string? OriginalFileName { get; init; }

   public required string RoleSlug { get; init; }

   public required string RoleDisplayName { get; init; }

   public string? Caption { get; init; }

   public string? AltText { get; init; }

   public int SortOrder { get; init; }

   public long SizeBytes { get; init; }

   public int? WidthPixels { get; init; }

   public int? HeightPixels { get; init; }

   private static string GetRoleSlug(SiteAssetRole role)
   {
      return role switch
      {
         SiteAssetRole.Attachment => "attachment",
         SiteAssetRole.HeroImage => "hero-image",
         SiteAssetRole.InlineImage => "inline-image",
         SiteAssetRole.SocialImage => "social-image",
         SiteAssetRole.GeneratedPdf => "generated-pdf",
         SiteAssetRole.Download => "download",
         _ => throw new InvalidOperationException(
            $"Unsupported site asset role: {role}."),
      };
   }

   private static string GetRoleDisplayName(SiteAssetRole role)
   {
      return role switch
      {
         SiteAssetRole.Attachment => "Attachment",
         SiteAssetRole.HeroImage => "Hero Image",
         SiteAssetRole.InlineImage => "Inline Image",
         SiteAssetRole.SocialImage => "Social Image",
         SiteAssetRole.GeneratedPdf => "Generated PDF",
         SiteAssetRole.Download => "Download",
         _ => throw new InvalidOperationException(
            $"Unsupported site asset role: {role}."),
      };
   }
}