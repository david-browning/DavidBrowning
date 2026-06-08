// Copyright Â© 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Admin.ViewModels.Work;

public class CredentialEditViewModel
{
   public required EditModes EditMode { get; init; }

   public CredentialEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public CredentialEditViewModel(Credential cred)
   {
      EditMode = EditModes.Edit;
   }
}
