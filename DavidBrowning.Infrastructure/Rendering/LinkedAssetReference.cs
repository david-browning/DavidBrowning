// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Infrastructure.Rendering;

public sealed class LinkedAssetReference
{
   public required string ReferenceKey { get; init; }

   public required string AssetKey { get; init; }

   public string? AltText { get; init; }

   public string? Caption { get; init; }
}