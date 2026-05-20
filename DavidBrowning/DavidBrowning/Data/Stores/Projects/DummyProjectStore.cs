// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Data.Stores.Projects
{
   internal sealed class DummyProjectStore : IProjectStore
   {
      public Task<IReadOnlyList<Project>> GetPublishedProjectsAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<Project>> GetFeaturedProjectsAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<Project?> GetPublishedProjectBySlugAsync(string slug, CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<Project>> GetPublishedProjectsByTagSlugAsync(string tagSlug, CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<Project>> GetPublishedProjectsByStackTagSlugAsync(string stackTagSlug, CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<ProjectTag>> GetProjectTagsAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<ProjectStackTag>> GetProjectStackTagsAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<ProjectStatus>> GetProjectStatusesAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<ProjectType>> GetProjectTypesAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<ProjectOrigin>> GetProjectOriginsAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<ProjectVisibility>> GetProjectVisibilitiesAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }
   }
}
