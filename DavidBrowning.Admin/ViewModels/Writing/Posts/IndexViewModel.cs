// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing.Posts;

public class IndexViewModel
{
   public required PostMetadataViewModel Metadata { get; set; }

   public required IReadOnlyList<PostRevision> RevisionHistory { get; set; }

   public int? CurrentRevisionId { get; set; }

   public PostRevisionContentViewModel? RevisionContent { get; set; }
}
