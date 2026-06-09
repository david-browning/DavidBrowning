// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace DavidBrowning.Admin.Extensions;

public static class HttpExtensions
{
   public static void TriggerAdminOffcanvasClose(
      this HttpResponse response,
      string offcanvasId)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(offcanvasId);
      string eventJson = JsonSerializer.Serialize(
         new
         {
            adminOffcanvasClose = new
            {
               offcanvasId,
            },
         });

      response.Headers.Append("HX-Trigger-After-Swap", eventJson);
   }


   public static bool IsHtmxRequest(this HttpRequest request)
   {
      return string.Equals(request.Headers["HX-Request"], "true",
         StringComparison.OrdinalIgnoreCase);
   }
}