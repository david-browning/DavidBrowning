// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Models.ViewModels.Work;

public class CredentialViewModel
{
   [SetsRequiredMembers]
   public CredentialViewModel(Credential cred)
   {
      IssuingOrganization = cred.IssuingOrganization;
      Name = cred.Name;
      CredentialUrl = cred.CredentialUrl;
      DateDisplayText = cred.DateDisplayText;
      Description = cred.Description;
      Type = cred.Type;
   }

   public required string IssuingOrganization { get; init; }

   public required string Name { get; init; }

   public string? Type { get; init; }

   public string? DateDisplayText { get; init; }

   public string? Description { get; init; }

   public string? CredentialUrl { get; init; }
}
