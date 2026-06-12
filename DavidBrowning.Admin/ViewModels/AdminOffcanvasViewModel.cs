// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Admin.ViewModels;

public sealed class AdminOffcanvasViewModel
{
   public required string Id { get; init; }

   public required string Title { get; init; }

   public string? CssClass { get; set; }

   public string Placeholder { get; init; } = "Select an item to edit.";

   public string LoadingText { get; init; } = "Loading...";

   public string BodyId => $"{Id}-body";

   public string OverlayId => $"{Id}-overlay";

   public string TitleId => $"{Id}-title";
}