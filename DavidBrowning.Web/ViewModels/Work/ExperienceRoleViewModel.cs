// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Web.ViewModels.Work;

public sealed class ExperienceRoleViewModel
{
   [SetsRequiredMembers]
   public ExperienceRoleViewModel(ExperienceRole role)
   {
      DateDisplayText = role.DateDisplayText;
      Title = role.Title;
      Description = role.Description;
      Bullets = role.Bullets.Select(b => b.Text).ToList();
   }

   public string? DateDisplayText { get; init; }

   public required string Title { get; init; }

   public string? Description { get; init; }

   public required IReadOnlyList<string> Bullets { get; init; }
}
