// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Helpers;

public static class StringExtensions
{
   public static bool EqualsOrdinalIgnoreCase(
      this string? value,
      string? other)
   {
      return string.Equals(value, other, StringComparison.OrdinalIgnoreCase);
   }
}