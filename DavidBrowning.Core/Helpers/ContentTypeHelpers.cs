// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Helpers;

public static class ContentTypeHelpers
{
   public static bool IsImageContentType(string? contentType)
   {
      return contentType?.StartsWith(
         "image/", StringComparison.OrdinalIgnoreCase) == true;
   }

   public static bool IsTextContentType(string? contentType)
   {
      if (contentType?.StartsWith(
         "text/", StringComparison.OrdinalIgnoreCase) == true)
      {
         return true;
      }

      return contentType.EqualsOrdinalIgnoreCase("application/json") ||
         contentType.EqualsOrdinalIgnoreCase("application/xml") ||
         contentType.EqualsOrdinalIgnoreCase("application/javascript") ||
         contentType.EqualsOrdinalIgnoreCase("application/x-javascript") ||
         contentType.EqualsOrdinalIgnoreCase("image/svg+xml");
   }

   public static bool IsRasterImageContentType(string? contentType)
   {
      return contentType.EqualsOrdinalIgnoreCase("image/png") ||
         contentType.EqualsOrdinalIgnoreCase("image/jpeg") ||
         contentType.EqualsOrdinalIgnoreCase("image/gif") ||
         contentType.EqualsOrdinalIgnoreCase("image/webp");
   }
}