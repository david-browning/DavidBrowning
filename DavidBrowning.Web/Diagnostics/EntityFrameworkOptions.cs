// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Web.Diagnostics;

public class EntityFrameworkOptions
{
   public bool EnableSensitiveDataLogging { get; set; } = false;

   public bool EnableDetailedErrors { get; set; } = false;

   public bool EnableSqlCommandLogging { get; set; } = false;
}
