// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Globalization;
using System.Text.RegularExpressions;

namespace DavidBrowning.Helpers;

public static partial class FontAwesomeHelpers
{
   public static string GetDisplayName(
      string iconCssClass)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(iconCssClass);
      Match match = IconNamePattern().Match(iconCssClass);

      if (!match.Success)
      {
         throw new ArgumentException(
            $"Could not extract a Font Awesome icon name from " +
            $"'{iconCssClass}'.",
            nameof(iconCssClass));
      }

      string iconName = match.Groups["name"].Value;
      return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(
         iconName.Replace('-', ' '));
   }

   [GeneratedRegex(
      @"^fa-(?:solid|regular|brands)\s+fa-(?<name>[a-z0-9-]+)(?:\s|$)",
      RegexOptions.CultureInvariant |
      RegexOptions.IgnoreCase)]
   private static partial Regex IconNamePattern();
}