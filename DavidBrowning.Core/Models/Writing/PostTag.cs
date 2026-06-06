// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Models.Writing;

public sealed class PostTag
{
   public int PostId { get; set; }

   public Post? Post { get; set; }

   public int WritingTagId { get; set; }

   public WritingTag? WritingTag { get; set; }
}
