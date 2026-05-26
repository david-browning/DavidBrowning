// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Models;

/// <summary>
/// How the site should present the writing content.
/// </summary>
public enum RenderMode : byte
{
   Article = 0,
   TechnicalNote = 1,
   CodeHeavy = 2,
   Longform = 3,
   Paper = 4,
   Gallery = 5,
   Markdown = 6,
}
