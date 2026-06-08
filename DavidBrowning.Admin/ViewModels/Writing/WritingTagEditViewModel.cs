// Copyright Â© 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class WritingTagEditViewModel
{
   public required EditModes EditMode { get; init; }

   [Required]
   public int? Id { get; set; }

   /// <summary>
   /// The content of the tag. Something like ".NET" or "Thoughts".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? DisplayName { get; set; }

   /// <summary>
   /// The URL-friendly text so we can query all posts with this tag applied.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(DataConstants.SlugRegex, 
      ErrorMessage = DataConstants.SlugRegexError)]
   public string? Slug { get; set; }

   public WritingTagEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public WritingTagEditViewModel(WritingTag tag)
   {
      EditMode = EditModes.Edit;
      Slug = tag.Slug;
      Id = tag.Id;
      DisplayName = tag.DisplayName;
   }
}
