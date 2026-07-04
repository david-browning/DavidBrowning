// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using DavidBrowning.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Admin;

public static class Program
{
   public static void Main(string[] args)
   {
      var builder = WebApplication.CreateBuilder(args);

      builder.Logging.ClearProviders();
      builder.Logging.AddConsole();
      builder.Logging.AddDebug();

      builder.Services.AddDavidBrowningInfrastructure(
         builder.Configuration,
         builder.Environment);

      //builder.Services.AddAdminAuthoringServices();
      builder.Services.AddControllersWithViews();

      var app = builder.Build();

      app.ConfigureErrorHandling(builder.Configuration);

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      app.MapControllerRoute(
         name: ConfigurationHelpers.DefaultRouteName,
         pattern: ConfigurationHelpers.DefaultRoutePattern);

      app.Run();
   }
}