// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Infrastructure.Options;

public class AzureBlobContentStoreOptions
{
   public string? ConnectionName { get; set; }

   public string? ConnectionString { get; set; }

   public string ContainerName { get; set; } = "content";
}