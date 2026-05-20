// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using DavidBrowning.Models.Writing;

namespace DavidBrowning.Models
{
   /// <summary>
   /// Maps to db_SiteAssetLinks.
   /// Connects a writing post to a reusable site asset.
   /// </summary>
   public sealed class SiteAssetLink
   {
      /// <summary>
      /// Foreign key to db_Posts.
      /// </summary>
      public int PostId { get; set; }

      public Post? Post { get; set; }

      /// <summary>
      /// Foreign key to db_SiteAssets.
      /// </summary>
      public int SiteAssetId { get; set; }

      public SiteAsset? SiteAsset { get; set; }

      /// <summary>
      /// Describes how the post uses this asset.
      /// </summary>
      public SiteAssetRole Role { get; set; }

      /// <summary>
      /// Manual display ordering for assets with the same role.
      /// </summary>
      public int SortOrder { get; set; }
   }
}