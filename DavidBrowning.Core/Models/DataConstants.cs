// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Models;

public sealed class DataConstants
{
   public const int MaxSlugLength = 64;
   public const int MaxLabelLength = 128;
   public const int MaxAssetKeyLength = 256;
   public const int MaxMetadataLength = 512;
   public const int MaxInterestSummaryLength = 768;
   public const int MaxNameLength = 256;
   public const int MaxUrlLength = 2048;
   public const int MaxIconCssClassLength = 128;
   public const int MaxContentTypeLength = 128;

   public const string SlugRegex = @"^[a-z0-9]+(?:-[a-z0-9]+)*$";
   public const string SlugRegexError = 
      "Use lowercase letters, numbers, and hyphens only.";
}