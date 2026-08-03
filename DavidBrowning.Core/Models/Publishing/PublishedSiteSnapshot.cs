// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Models.Publishing;

public sealed class PublishedSiteSnapshot
{
   public int SchemaVersion { get; init; } = 1;

   public required string Version { get; init; }

   public required DateTimeOffset PublishedAtUtc { get; init; }

   public IReadOnlyList<PublishedProject> Projects { get; init; } =
      Array.Empty<PublishedProject>();

   public IReadOnlyList<PublishedWriting> Writings { get; init; } =
      Array.Empty<PublishedWriting>();

   public IReadOnlyList<PublishedExperience> Experience { get; init; } =
      Array.Empty<PublishedExperience>();

   public IReadOnlyList<PublishedCredential> Credentials { get; init; } =
      Array.Empty<PublishedCredential>();

   public IReadOnlyList<PublishedInterest> Interests { get; init; } =
      Array.Empty<PublishedInterest>();

   public IReadOnlyList<PublishedLookup> AllProjectStacks { get; init; } =
      Array.Empty<PublishedLookup>();

   public IReadOnlyList<PublishedLookup> AllProjectProjectTags { get; init; }
      = Array.Empty<PublishedLookup>();

   public IReadOnlyList<PublishedLookup> AllWritingTags { get; init; }
      = Array.Empty<PublishedLookup>();

   public PublishedSiteSnapshot()
   {

   }
}
