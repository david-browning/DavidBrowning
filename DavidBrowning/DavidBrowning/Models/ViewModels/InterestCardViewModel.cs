// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Models.ViewModels;

public sealed class InterestCardViewModel
{
   public required string Title { get; init; }

   public required string Description { get; init; }

   public string? IconCssClass { get; init; }

   public RenderedContent? Image { get; init; }
}
