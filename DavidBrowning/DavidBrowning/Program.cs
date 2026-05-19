// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Data.Stores.Writing;
using DavidBrowning.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DavidBrowning
{
   public static partial class Program
   {
      private const string _backupStoreName = "Dummy";

      public static void Main(string[] args)
      {
         var builder = WebApplication.CreateBuilder(args);
         ConfigureLogging(builder);
         ConfigureServices(builder);
         ConfigureDatabase(builder);
         ConfigureAuthorization(builder);

         var app = builder.Build();
         ConfigureWebApp(app);
         app.Run();
      }

      private static void ConfigureLogging(WebApplicationBuilder builder)
      {
         builder.Logging.ClearProviders();
         builder.Logging.AddConsole();
         builder.Logging.AddDebug();
         //logger.AddAzureWebAppDiagnostics();
      }

      private static void ConfigureServices(WebApplicationBuilder builder)
      {
         builder.Services.AddControllersWithViews();
      }

      private static void ConfigureDatabase(WebApplicationBuilder builder)
      {
         var errorProvider = builder.Configuration["Stores:ErrorStore:Provider"] ?? _backupStoreName;
         if (string.Equals(errorProvider, _backupStoreName, StringComparison.OrdinalIgnoreCase))
         {
            builder.Services.AddSingleton<IErrorStore, DummyErrorStore>();
         }
         else if (string.Equals(errorProvider, "AzureSql", StringComparison.OrdinalIgnoreCase))
         {
            throw new InvalidOperationException("Database not supported yet.");
         }
         else
         {
            throw new InvalidOperationException(
               $"The writing store provider {errorProvider} is unknown.");
         }

         var writingProvider = builder.Configuration["Stores:WritingStore:Provider"] ?? _backupStoreName;
         if (string.Equals(writingProvider, _backupStoreName, StringComparison.OrdinalIgnoreCase))
         {
            builder.Services.AddSingleton<IWritingStore, DummyWritingStore>();
         }
         else if (string.Equals(writingProvider, "AzureSql", StringComparison.OrdinalIgnoreCase))
         {
            throw new InvalidOperationException("Database not supported yet.");
         }
         else
         {
            throw new InvalidOperationException(
               $"The writing store provider {writingProvider} is unknown.");
         }
      }

      private static void ConfigureAuthorization(WebApplicationBuilder builder)
      {

      }

      private static void ConfigureWebApp(WebApplication app)
      {
         // Configure the HTTP request pipeline.
         if (!app.Environment.IsDevelopment())
         {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this
            // for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
         }

         app.UseMiddleware<ErrorLoggingMiddleware>();

         app.UseHttpsRedirection();
         app.UseStaticFiles();
         app.UseRouting();

         app.UseAuthentication();
         app.UseAuthorization();

         //app.UseMiddleware<DatabaseExceptionLogger>();

         app.MapControllerRoute(
             name: "default",
             pattern: "{controller=Home}/{action=Index}/{id?}");
      }
   }
}