// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Projects;
using DavidBrowning.Models.Work;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Models.Published;

public sealed class PublishedSiteSnapshot
{
   public int SchemaVersion { get; set; } = 1;

   public required string Version { get; set; }

   public required DateTimeOffset PublishedAtUtc { get; set; }

   public IReadOnlyList<PublishedProject> Projects { get; set; } =
      Array.Empty<PublishedProject>();

   public IReadOnlyList<PublishedWriting> Writings { get; set; } =
      Array.Empty<PublishedWriting>();

   public IReadOnlyList<PublishedExperience> Experience { get; set; } =
      Array.Empty<PublishedExperience>();

   public IReadOnlyList<PublishedCredential> Credentials { get; set; } =
      Array.Empty<PublishedCredential>();

   public IReadOnlyList<PublishedInterest> Interests { get; set; } =
      Array.Empty<PublishedInterest>();

   public PublishedSiteSnapshot()
   {

   }

   [SetsRequiredMembers]
   public PublishedSiteSnapshot(
      string version,
      DateTimeOffset publishedAtUtc,
      IEnumerable<Project> projects,
      IEnumerable<Post> writings,
      IEnumerable<Experience> experience,
      IEnumerable<Credential> credentials,
      IEnumerable<Interest> interests)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(version);
      ArgumentNullException.ThrowIfNull(projects);
      ArgumentNullException.ThrowIfNull(writings);
      ArgumentNullException.ThrowIfNull(experience);
      ArgumentNullException.ThrowIfNull(credentials);
      ArgumentNullException.ThrowIfNull(interests);

      Version = version;
      PublishedAtUtc = publishedAtUtc;
      Projects = projects
         .OrderBy(project => project.SortOrder)
         .ThenBy(project => project.Name)
         .Select(project => new PublishedProject(project))
         .ToArray();
      Writings = writings
         .OrderByDescending(writing => writing.PublishedDateUtc)
         .ThenBy(writing => writing.Title)
         .Select(writing => new PublishedWriting(writing))
         .ToArray();
      Experience = experience
         .Where(value => value.IsActive)
         .OrderBy(value => value.SortOrder)
         .Select(value => new PublishedExperience(value))
         .ToArray();
      Credentials = credentials
         .Where(value => value.IsActive)
         .OrderBy(value => value.SortOrder)
         .Select(value => new PublishedCredential(value))
         .ToArray();
      Interests = interests
         .Where(value => value.IsActive)
         .OrderBy(value => value.SortOrder)
         .Select(value => new PublishedInterest(value))
         .ToArray();
   }
}
