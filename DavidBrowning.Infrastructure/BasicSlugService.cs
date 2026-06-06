// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DavidBrowning.Infrastructure;

public sealed class BasicSlugService : ISlugService
{
   public string CreateSlug(string value)
   {
      if (string.IsNullOrWhiteSpace(value))
      {
         return string.Empty;
      }

      string normalized = RemoveDiacritics(value).ToLowerInvariant().Trim();
      normalized = ReplaceSymbolWords(normalized);

      // Remove any remaining invalid characters.
      normalized = _invalidCharactersRegex.Replace(normalized, string.Empty);

      // Replace whitespace and repeated dashes.
      normalized = _whitespaceRegex.Replace(normalized, "-");

      // Trim extra dashes.
      normalized = normalized.Trim('-');

      return normalized;
   }

   public string CleanSlug(string slug)
   {
      return slug.Trim().ToLowerInvariant();
   }

   private static string RemoveDiacritics(string value)
   {
      string normalized = value.Normalize(NormalizationForm.FormD);
      StringBuilder builder = new(capacity: normalized.Length);
      foreach (char character in normalized)
      {
         var category = CharUnicodeInfo.GetUnicodeCategory(character);
         if (category != UnicodeCategory.NonSpacingMark)
         {
            builder.Append(character);
         }
      }

      return builder.ToString().Normalize(NormalizationForm.FormC);
   }

   private static string ReplaceSymbolWords(string value)
   {
      StringBuilder builder = new(capacity: value.Length);
      foreach (char character in value)
      {
         if (_symbolWords.TryGetValue(character, out string? word))
         {
            builder.Append('-');
            builder.Append(word);
            builder.Append('-');
         }
         else
         {
            builder.Append(character);
         }
      }

      return builder.ToString();
   }

   private static readonly IReadOnlyDictionary<char, string> _symbolWords =
      new Dictionary<char, string>
      {
         ['.'] = "dot",
         ['#'] = "sharp",
         ['+'] = "plus",
         ['&'] = "and",
         ['@'] = "at",
      };

   private static readonly Regex _invalidCharactersRegex = new(
     pattern: @"[^a-z0-9\s-]",
     options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

   private static readonly Regex _whitespaceRegex = new(
      pattern: @"[\s-]+",
      options: RegexOptions.Compiled | RegexOptions.CultureInvariant);
}
