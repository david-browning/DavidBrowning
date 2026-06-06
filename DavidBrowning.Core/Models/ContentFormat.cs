// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Models;

/// <summary>
/// Describes the source syntax used by authored content.
/// </summary>
public enum ContentFormat : byte
{
   PlainText = 0,
   Markdown = 1,
   Html = 2,
}
