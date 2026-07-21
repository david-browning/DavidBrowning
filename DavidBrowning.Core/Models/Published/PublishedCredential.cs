// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Models.Published;

public sealed record PublishedCredential
{
   public required string Name { get; init; }

   public required string Issuer { get; init; }

   public string? Description { get; init; }

   public string? Url { get; init; }

   public int SortOrder { get; init; }

   public PublishedCredential()
   {

   }

   [SetsRequiredMembers]
   public PublishedCredential(Credential c)
   {
      Name = c.Name;
      Issuer = c.IssuingOrganization;
      Description = c.Description;
      Url = c.CredentialUrl;
      SortOrder = c.SortOrder;
   }
}
