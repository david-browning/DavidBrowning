// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Diagnostics
{
   public class DiagnosticsOptions
   {
      public bool EnableDebugEndpoints { get; set; } = false;

      public EntityFrameworkOptions EntityFrameworkOptions { get; set; } = new();
   }
}
