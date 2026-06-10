// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;

namespace DavidBrowning.Admin.ViewModels.Work;

public sealed class IndexViewModel
{
   public Resume.IndexViewModel? Resume { get; set; }

   public IReadOnlyList<Experience.PreviewViewModel> ExperiencePreview { get; set; } =
      new List<Experience.PreviewViewModel>();

   public IReadOnlyList<Credentials.PreviewViewModel> CredentialPreview { get; set; } =
      new List<Credentials.PreviewViewModel>();
}