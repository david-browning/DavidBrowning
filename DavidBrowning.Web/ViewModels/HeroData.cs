// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Text.Json.Serialization;

namespace DavidBrowning.Web.ViewModels;

public class HeroData
{
   [JsonPropertyName("title")]
   public string? Title { get; set; }

   [JsonPropertyName("subtitle")]
   public string? Subtitle { get; set; }

   [JsonPropertyName("lede")]
   public string? Lede { get; set; }
}
