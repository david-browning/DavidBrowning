// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Web.ViewModels.Work;

public sealed class ExperienceViewModel
{
   [SetsRequiredMembers]
   public ExperienceViewModel(Experience exp)
   {
      CompanyName = exp.CompanyName;
      LocationDisplayText = exp.LocationDisplayText;
      Roles = exp.Roles.Select(r => new ExperienceRoleViewModel(r)).ToList();
   }

   public required string CompanyName { get; init; }

   /// <summary>
   /// City/state or remote.
   /// </summary>
   public string? LocationDisplayText { get; init; }

   public required IReadOnlyList<ExperienceRoleViewModel> Roles { get; init; }
}
