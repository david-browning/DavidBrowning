// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using DavidBrowning.Infrastructure.Assets;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.Models.Published;

namespace DavidBrowning.Infrastructure.Publishing;

public sealed class PublicSiteSnapshotBuilder : IPublicSiteSnapshotBuilder
{
   public PublicSiteSnapshotBuilder(
      IProjectStore projectStore,
      IWritingStore writingStore,
      IWorkStore workStore,
      IUncategorizedStore uncategorizedStore,
      IContentStore contentStore)
   {
      _projectStore = projectStore;
      _writingStore = writingStore;
      _workStore = workStore;
      _uncategorizedStore = uncategorizedStore;
      _contentStore = contentStore;
   }

   public async Task<PublishedSiteSnapshot> BuildAsync(
      string version, 
      DateTimeOffset publishedAtUtc, 
      CancellationToken cancellationToken = default)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(version);

      // Keep SQL operations sequential. These stores normally share one
      // scoped DbContext, which must not execute concurrent operations.
      var projects = await BuildProjectsAsync(cancellationToken);
      var writings = await BuildWritingsAsync(cancellationToken);
      var experience = await BuildExperienceAsync(cancellationToken);
      var credentials = await BuildCredentialsAsync(cancellationToken);
      var interests = await BuildInterestsAsync(cancellationToken);

      return new PublishedSiteSnapshot
      {
         Version = version,
         PublishedAtUtc = publishedAtUtc,
         Projects = projects,
         Writings = writings,
         Experience = experience,
         Credentials = credentials,
         Interests = interests,
      };
   }

   private async Task<IReadOnlyList<PublishedProject>> BuildProjectsAsync(
      CancellationToken cancellationToken)
   {

   }

   private async Task<IReadOnlyList<PublishedWriting>> BuildWritingsAsync(
      CancellationToken cancellationToken)
   {

   }

   private async Task<IReadOnlyList<PublishedExperience>> BuildExperienceAsync(
      CancellationToken cancellationToken)
   {
      var values = await _workStore.GetExperienceAsync(cancellationToken);
      return values.Select(e => new PublishedExperience(e)).ToList();
   }

   private async Task<IReadOnlyList<PublishedCredential>> BuildCredentialsAsync(
      CancellationToken cancellationToken)
   {
      var values = await _workStore.GetCredentialsAsync(cancellationToken);
      return values.Select(c => new PublishedCredential(c)).ToList();
   }

   private async Task<IReadOnlyList<PublishedInterest>> BuildInterestsAsync(
      CancellationToken cancellationToken)
   {
      var values = await _uncategorizedStore.GetInterestsAsync(
         cancellationToken);
      return values.Select(i => new PublishedInterest(i)).ToList();
   }

   private readonly IProjectStore _projectStore;
   private readonly IWritingStore _writingStore;
   private readonly IWorkStore _workStore;
   private readonly IUncategorizedStore _uncategorizedStore;
   private readonly IContentStore _contentStore;
}
