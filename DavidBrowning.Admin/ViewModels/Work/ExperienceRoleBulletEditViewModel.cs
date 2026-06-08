// Copyright Â© 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Admin.ViewModels.Work;

public class ExperienceRoleBulletEditViewModel
{
   public required EditModes EditMode { get; init; }

   public ExperienceRoleBulletEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public ExperienceRoleBulletEditViewModel(ExperienceRoleBullet bullet)
   {
      EditMode = EditModes.Edit;
   }
}
