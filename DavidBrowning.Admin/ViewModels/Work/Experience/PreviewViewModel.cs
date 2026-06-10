// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Admin.ViewModels.Work.Experience;

public sealed class PreviewViewModel
{
   public required int Id { get; set; }

   public required string CompanyName { get; set; }

   public ICollection<ExperienceRole>? Roles { get; set; }
}
