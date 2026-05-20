// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DavidBrowning.Services.Slugs
{
   internal sealed class BasicSlugService : ISlugService
   {
      public string CreateSlug(string value)
      {
         if (string.IsNullOrWhiteSpace(value))
         {
            return string.Empty;
         }

         string normalized = RemoveDiacritics(value)
            .ToLowerInvariant()
            .Trim();

         // Remove any invalid characters
         normalized = _invalidCharactersRegex.Replace(normalized, string.Empty);

         // Replace the whitespace
         normalized = _whitespaceRegex.Replace(normalized, "-");

         // Trim extra dashes
         normalized = normalized.Trim('-');

         return normalized;
      }

      private static string RemoveDiacritics(string value)
      {
         string normalized = value.Normalize(NormalizationForm.FormD);
         StringBuilder builder = new(capacity: normalized.Length);

         foreach (char character in normalized)
         {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(character);

            if (category != UnicodeCategory.NonSpacingMark)
            {
               builder.Append(character);
            }
         }

         return builder.ToString().Normalize(NormalizationForm.FormC);
      }

      private static readonly Regex _invalidCharactersRegex = new(
        pattern: @"[^a-z0-9\s-]",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

      private static readonly Regex _whitespaceRegex = new(
         pattern: @"[\s-]+",
         options: RegexOptions.Compiled | RegexOptions.CultureInvariant);
   }
}
