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
      AltText = link.AltTextOverride ?? asset.AltText;
      Caption = link.Caption;
      RoleSlug = role.Slug;
      WidthPixels = asset.WidthPixels;
      HeightPixels = asset.HeightPixels;
   }

   [SetsRequiredMembers]
   public AssetBlockViewModel(PostAssetLink link)
   {
      var asset = link.SiteAsset ??
         throw new InvalidOperationException(
            "Post asset link is missing its site asset.");

      AssetKey = asset.AssetKey;
      ContentType = asset.ContentType;
      AltText = link.AltTextOverride ?? asset.AltText;
      Caption = link.Caption;
      RoleSlug = GetRoleSlug(link.Role);
      WidthPixels = asset.WidthPixels;
      HeightPixels = asset.HeightPixels;
   }

   public required string AssetKey { get; init; }

   public required string ContentType { get; init; }

   public required string RoleSlug { get; init; }

   public string? Caption { get; init; }

   public string? AltText { get; init; }

   public int? WidthPixels { get; init; }

   public int? HeightPixels { get; init; }

   private static string GetRoleSlug(PostAssetRole role)
   {
      return role switch
      {
         PostAssetRole.Attachment => "attachment",
         PostAssetRole.HeroImage => "hero-image",
         PostAssetRole.InlineImage => "inline-image",
         PostAssetRole.SocialImage => "social-image",
         PostAssetRole.GeneratedPdf => "generated-pdf",
         PostAssetRole.Download => "download",
         _ => throw new InvalidOperationException(
            $"Unsupported site asset role: {role}."),
      };
   }
}