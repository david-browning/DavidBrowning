// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DavidBrowning.Web.ViewModels.Home;

public sealed class PrivacyViewModel
{
   [JsonPropertyName("Title")]
   public required string Title { get; init; }

   [JsonPropertyName("Description")]
   public required string Description { get; init; }

   [JsonPropertyName("LastUpdated")]
   public required string LastUpdateDisplayDate { get; init; }

   [JsonPropertyName("Sections")]
   public required List<HeroData> Sections { get; init; }

   public required SeoMetadataViewModel Seo { get; init; }
}
