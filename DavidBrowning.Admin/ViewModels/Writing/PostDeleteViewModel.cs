// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

namespace DavidBrowning.Admin.ViewModels.Writing;

public sealed class PostDeleteViewModel
{
   [Range(1, int.MaxValue)]
   public int Id { get; set; }

   [Required]
   public string? Title { get; set; }

   [Required]
   public string? Slug { get; set; }

   public int RevisionCount { get; set; }

   public int TagCount { get; set; }

   public int RelatedProjectCount { get; set; }
}
