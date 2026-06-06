// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Infrastructure;

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

   /// <summary>
   /// Cleans up the existing slug of any whitespace and makes it
   /// lowercase.
   /// 
   ///  _______________
   /// |_______________|
   ///  |O| | |O| |O| | O
   /// O O  O O  O O O O O
   /// 
   /// @     @
   ///  \   /
   ///  _\_/______________
   /// / wyeh, wyeh, wyeh \
   /// \__________________/
   ///
   /// </summary>
   /// <param name="slug">An existing slug either from the database 
   /// or made using CreateSlug</param>
   /// <returns></returns>
   string CleanSlug(string slug);
}