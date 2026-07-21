// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Models.Published;

public sealed class PublishedCredential
{
   public required string IssuingOrganization { get; set; }

   public required string Name { get; set; }

   public string? Type { get; set; }

   public string? DateDisplayText { get; set; }

   public string? Description { get; set; }

   public string? CredentialUrl { get; set; }

   public int SortOrder { get; set; }

   public PublishedCredential()
   {

   }

   [SetsRequiredMembers]
   public PublishedCredential(Credential credential)
   {
      ArgumentNullException.ThrowIfNull(credential);

      IssuingOrganization = credential.IssuingOrganization;
      Name = credential.Name;
      Type = credential.Type;
      DateDisplayText = credential.DateDisplayText;
      Description = credential.Description;
      CredentialUrl = credential.CredentialUrl;
      SortOrder = credential.SortOrder;
   }
}
