// Copyright Â© 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using DavidBrowning.Models.Error;
using DavidBrowning.ViewModels;

namespace DavidBrowning.Admin.ViewModels.Error;

public class IndexViewModel
{
   public required IReadOnlyList<WebsiteError> Errors { get; init; }

   public required PagerViewModel Pager { get; init; }
}
