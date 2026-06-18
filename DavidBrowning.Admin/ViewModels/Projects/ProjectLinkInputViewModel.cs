// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DavidBrowning.Models;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectLinkInputViewModel : IValidatableObject
{
   [DisplayName("Type")]
   public int? ProjectLinkTypeId { get; set; }

   [StringLength(DataConstants.MaxLabelLength)]
   public string? Label { get; set; }

   [StringLength(DataConstants.MaxUrlLength)]
   [Url]
   public string? Url { get; set; }

   public int SortOrder { get; set; }

   public ProjectLinkInputViewModel()
   {
   }

   public ProjectLinkInputViewModel(ProjectLink link)
   {
      ProjectLinkTypeId = link.ProjectLinkTypeId;
      Label = link.Label;
      Url = link.Url;
      SortOrder = link.SortOrder;
   }

   public bool IsEmpty()
   {
      return ProjectLinkTypeId is null &&
         string.IsNullOrWhiteSpace(Label) &&
         string.IsNullOrWhiteSpace(Url);
   }

   public IEnumerable<ValidationResult> Validate(
      ValidationContext validationContext)
   {
      if (IsEmpty())
      {
         yield break;
      }

      if (ProjectLinkTypeId is null)
      {
         yield return new ValidationResult(
            "Select a link type.", new[] { nameof(ProjectLinkTypeId) });
      }

      if (string.IsNullOrWhiteSpace(Label))
      {
         yield return new ValidationResult(
            "Enter a link label.", new[] { nameof(Label) });
      }

      if (string.IsNullOrWhiteSpace(Url))
      {
         yield return new ValidationResult(
            "Enter a link URL.", new[] { nameof(Url) });
      }
   }

   public ProjectLink ToProjectLink(int sortOrder)
   {
      ArgumentNullException.ThrowIfNull(ProjectLinkTypeId);
      ArgumentException.ThrowIfNullOrWhiteSpace(Label);
      ArgumentException.ThrowIfNullOrWhiteSpace(Url);

      return new ProjectLink()
      {
         ProjectLinkTypeId = ProjectLinkTypeId.Value,
         Label = Label,
         Url = Url,
         SortOrder = sortOrder,
      };
   }
}