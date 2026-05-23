// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Services.Slugs;

namespace DavidBrowning.Services
{
   public sealed class UrlBuilder
   {
      public UrlBuilder(ISlugService slugService) 
      { 
         _slugService = slugService;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="slug">
      /// The slug can be a display name. It is automatically formatted 
      /// before the URL is returned.
      /// </param>
      /// <param name="controller"></param>
      /// <param name="endpoint"></param>
      /// <returns></returns>
      public string GetDetailsUrl(
         string slug,
         string controller, 
         string endpoint = "details")
      {
         return $"{controller}/{endpoint}/{_slugService.CreateSlug(slug)}";
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="slug">
      /// The slug can be a display name. It is automatically formatted 
      /// before the URL is returned.
      /// </param>
      /// <param name="controller"></param>
      /// <param name="endpoint"></param>
      /// <returns></returns>
      public string GetSearchUrl(
         string slug,
         string controller,
         string endpoint = "search")
      {
         return $"{controller}/{endpoint}/{_slugService.CreateSlug(slug)}";
      }

      private readonly ISlugService _slugService;
   }
}
