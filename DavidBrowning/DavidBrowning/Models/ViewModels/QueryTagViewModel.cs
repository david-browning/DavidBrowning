// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Models.ViewModels
{
   /// <summary>
   /// The slug is automatically calculated from the display name using
   /// the injected slug service. There is no need to format the display
   /// name to be URL friendly.
   /// </summary>
   public class QueryTagViewModel
   {
      public required string DisplayName { get; set; }

      public required string Controller { get; set; }

      public required string Endpoint { get; set; }

      public required string Slug { get; set; }
   }
}
