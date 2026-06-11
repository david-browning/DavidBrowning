// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;

namespace DavidBrowning.Admin.ViewModels.Work.Experience;

public class RoleEditListViewModel
{
   public required RoleEditViewModel Create { get; set; }

   public required ReorderListViewModel Roles { get; set; }
}
