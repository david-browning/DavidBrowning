// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Infrastructure.Data;

public sealed class DuplicateSlugException : InvalidOperationException
{
   public DuplicateSlugException(string slug)
      : base($"The slug '{slug}' is already in use.")
   {
      Slug = slug;
   }

   public string Slug { get; }
}