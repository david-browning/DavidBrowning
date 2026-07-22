// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Admin.ViewModels.Publish;

public enum PublishResultStatus
{
   None,
   Success,
   Error,
}

public sealed class IndexViewModel
{
   public string? ResultMessage { get; set; }

   public PublishResultStatus ResultStatus { get; set; }
}
