// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Services;

public sealed class UrlBuilder
{
   /// <summary>
   /// 
   /// </summary>
   /// <param name="slug">
   /// Assume a normalized slug
   /// </param>
   /// <param name="controller"></param>
   /// <param name="endpoint"></param>
   /// <returns></returns>
   public string GetDetailsUrl(
      string slug,
      string controller,
      string endpoint = "details")
   {
      return BuildSlugUrl(slug, controller, endpoint);
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="slug">
   /// Assume a normalized slug
   /// </param>
   /// <param name="controller"></param>
   /// <param name="endpoint"></param>
   /// <returns></returns>
   public string GetSearchUrl(
      string slug,
      string controller,
      string endpoint = "search")
   {
      return BuildSlugUrl(slug, controller, endpoint);
   }

   private static string BuildSlugUrl(
      string slug,
      string controller,
      string endpoint)
   {
      return string.Join(
         "/",
         string.Empty,
         controller.Trim('/'),
         endpoint.Trim('/'),
         slug.Trim('/'));
   }
}
