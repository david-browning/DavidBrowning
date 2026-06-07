// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Infrastructure;

public sealed class UrlBuilder
{
   public UrlBuilder()
   {
      _publicOrigin = new Uri("https://davidbrowning.com/", UriKind.Absolute);
   }

   public string GetAbsoluteUrl(string relativeUrl)
   {
      ArgumentException.ThrowIfNullOrEmpty(relativeUrl);
      return new Uri(_publicOrigin, relativeUrl.TrimStart('/')).AbsoluteUri;
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
   public string GetFilterUrl(
      string slug,
      string controller,
      string endpoint = "search")
   {
      return BuildSlugUrl(slug, controller, endpoint);
   }

   public string GetContentUrl(
      string assetKey,
      string controller = "content")
   {
      return BuildSlugUrl(assetKey, controller, "get");
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

   private readonly Uri _publicOrigin;
}
