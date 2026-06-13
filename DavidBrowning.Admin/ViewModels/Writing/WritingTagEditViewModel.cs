// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations;
using DavidBrowning.Models.Writing;
using DavidBrowning.Models;
using System;

namespace DavidBrowning.Admin.ViewModels.Writing;

public sealed class WritingTagEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   [Display(Name = "Display Name")]
   public string? DisplayName { get; set; }

   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(DataConstants.SlugRegex,
      ErrorMessage = DataConstants.SlugRegexError)]
   public string? Slug { get; set; }

   public WritingTagEditViewModel()
   {

   }

   public WritingTagEditViewModel(WritingTag tag)
   {
      EditMode = EditModes.Edit;
      Id = tag.Id;
      DisplayName = tag.DisplayName;
      Slug = tag.Slug;
   }

   public WritingTag ToTag()
   {
      ArgumentNullException.ThrowIfNullOrWhiteSpace(DisplayName);
      ArgumentNullException.ThrowIfNullOrWhiteSpace(Slug);

      return new WritingTag()
      {
         Id = Id ?? 0,
         DisplayName = DisplayName,
         Slug = Slug,
      };
   }
}
