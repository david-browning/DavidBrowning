// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Models.Writing
{
   public sealed class PostAssetLink
   {
      public int PostId { get; set; }

      public Post? Post { get; set; }

      public int PostAssetId { get; set; }

      public PostAsset? PostAsset { get; set; }

      public PostAssetRole Role { get; set; }

      public int SortOrder { get; set; }
   }
}
