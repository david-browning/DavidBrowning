// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

namespace DavidBrowning.Admin.ViewModels.Writing;

public sealed class PostRevisionDeleteModel
{
   [Range(1, int.MaxValue)]
   public int Id { get; set; }

   [Range(1, int.MaxValue)]
   public int PostId { get; set; }

   [Range(1, int.MaxValue)]
   public int RevisionNumber { get; set; }

   public bool IsCurrentRevision { get; set; }

   public int AssetLinkCount { get; set; }
}
