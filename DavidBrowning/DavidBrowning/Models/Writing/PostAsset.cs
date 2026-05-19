// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Writing
{
   /// <summary>
   /// Maps to db_PostAssets
   /// The BlobContainer/BlobName must be a unique combination, otherwise you
   /// have duplicate content.
   /// </summary>
   [PrimaryKey("Id")]
   [Index(nameof(BlobContainer), nameof(BlobName), IsUnique = true)]
   public sealed class PostAsset
   {
      [Required, Key]
      public int Id { get; set; }

      /// <summary>
      /// The type of content the asset is.
      /// Instead of having it be a foreign key, we'll just map it to an enum
      /// in code.
      /// </summary>
      public required PostAssetType AssetType { get; set; }

      [StringLength(DataConstants.MaxNameLength)]
      public string? OriginalFileName { get; set; }


      /// <summary>
      /// The Azure blob container that contains the content. Posts can use this
      /// and the BlobName to refer to the content.
      /// </summary>
      [StringLength(DataConstants.MaxAzureAssetLength)]
      public required string BlobContainer { get; set; }

      /// <summary>
      /// Name of the blob. Posts can use this to refer to the actual content.
      /// </summary>
      [StringLength(DataConstants.MaxAzureAssetLength)]
      public required string BlobName { get; set; }

      /// <summary>
      /// Alternate text of the asset. This will show up as a subtitle or when
      /// someone hovers the assets.
      /// </summary>
      public string? AltText { get; set; }

      public required long SizeBytes { get; set; }

      public required DateTime CreatedAtUtc { get; set; }

      public ICollection<PostAssetLink> PostLinks { get; } =
         new List<PostAssetLink>();
   }
}
