// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Web.ViewModels.Error;

public class StatusCodeViewModel
{
   public int HttpError { get; set; } = 500;

   public string? RequestId { get; set; }

   public string? OriginalPath { get; set; }

   public string? OriginalQueryString { get; set; }

   public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
