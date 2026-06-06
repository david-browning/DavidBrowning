// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Web.ViewModels.Work;

public sealed class IndexViewModel
{
   public required string PageTitle { get; init; }

   public required string HeroTitle { get; init; }

   /// <summary>
   /// The about me/career overview you'd see on LinkedIn or the resume.
   /// </summary>
   public required string Lede { get; init; }

   /// <summary>
   /// Places I've worked.
   /// </summary>
   public required IReadOnlyList<ExperienceViewModel> Experience { get; init; }

   /// <summary>
   /// Featured projects from work. No personal projects.
   /// </summary>
   public required IReadOnlyList<Project> FeaturedWorkProjects { get; init; }

   /// <summary>
   /// List of education credentials and certifications.
   /// </summary>
   public required IReadOnlyList<CredentialViewModel> Credentials { get; init; }
}
