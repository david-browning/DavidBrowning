// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Services.Slugs
{
   /// <summary>
   /// Creates slugs for web URLs.
   /// The slug service is not responsible for creating unique names. The 
   /// database enforces this and should be checked before a row is inserted.
   /// </summary>
   public interface ISlugService
   {
      /// <summary>
      /// Converts a string into a URL friendly text.
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      string CreateSlug(string value);
   }
}