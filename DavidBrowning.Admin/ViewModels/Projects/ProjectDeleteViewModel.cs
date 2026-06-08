// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectDeleteViewModel
{
   [Range(1, int.MaxValue)]
   public int Id { get; set; }

   [Required]
   public string? Name { get; set; }

   [Required]
   public string? Slug { get; set; }

   public int TagLinkCount { get; set; }

   public int StackTagLinkCount { get; set; }

   public int ExternalLinkCount { get; set; }

   public int AssetLinkCount { get; set; }

   public int RelatedPostCount { get; set; }
}
