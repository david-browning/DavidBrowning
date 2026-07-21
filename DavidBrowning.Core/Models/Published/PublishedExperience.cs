// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Models.Published;

public sealed class PublishedExperience
{
   public required string CompanyName { get; set; }

   public string? LocationDisplayText { get; set; }

   public int SortOrder { get; set; }

   public IReadOnlyList<PublishedExperienceRole> Roles { get; set; } =
      Array.Empty<PublishedExperienceRole>();

   public PublishedExperience()
   {

   }

   [SetsRequiredMembers]
   public PublishedExperience(Experience experience)
   {
      ArgumentNullException.ThrowIfNull(experience);

      CompanyName = experience.CompanyName;
      LocationDisplayText = experience.LocationDisplayText;
      SortOrder = experience.SortOrder;
      Roles = experience.Roles
         .Where(role => role.IsActive)
         .OrderBy(role => role.SortOrder)
         .Select(role => new PublishedExperienceRole(role))
         .ToArray();
   }
}

public sealed class PublishedExperienceRole
{
   public string? DateDisplayText { get; set; }

   public required string Title { get; set; }

   public string? Description { get; set; }

   public int SortOrder { get; set; }

   public IReadOnlyList<string> Bullets { get; set; } =
      Array.Empty<string>();

   public PublishedExperienceRole()
   {

   }

   [SetsRequiredMembers]
   public PublishedExperienceRole(ExperienceRole role)
   {
      ArgumentNullException.ThrowIfNull(role);

      DateDisplayText = role.DateDisplayText;
      Title = role.Title;
      Description = role.Description;
      SortOrder = role.SortOrder;
      Bullets = role.Bullets
         .Where(bullet => bullet.IsActive)
         .OrderBy(bullet => bullet.SortOrder)
         .Select(bullet => bullet.Text)
         .ToArray();
   }
}
