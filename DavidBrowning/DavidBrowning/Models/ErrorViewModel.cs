// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Models
{
   public class ErrorViewModel
   {
      public string? RequestId { get; set; }

      public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
   }
}
