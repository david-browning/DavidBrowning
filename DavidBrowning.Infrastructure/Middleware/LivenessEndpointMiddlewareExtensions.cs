using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DavidBrowning.Infrastructure.Middleware;

public static class LivenessEndpointMiddlewareExtensions
{
   public static IApplicationBuilder UseLivenessEndpoint(
      this IApplicationBuilder app,
      string path = "/healthz")
   {
      if (string.IsNullOrEmpty(path))
      {
         throw new ArgumentNullException(path);
      }

      if (!path.StartsWith("/"))
      {
         throw new ArgumentException("The endpoint must begin with a '/'");
      }

      PathString healthPath = new(path);
      return app.Use(async (context, next) =>
      {
         if (context.Request.Path.Equals(healthPath, StringComparison.OrdinalIgnoreCase))
         {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.Headers.CacheControl = "no-store";
            context.Response.Headers["X-Robots-Tag"] = "noindex, nofollow";

            // Do the minimal check if the app is working.


            // At this point, the app is up and running but this says nothing
            // about database health or blob health.
            await context.Response.WriteAsync("Healthy");

            return;
         }

         await next();
      });
   }
}
