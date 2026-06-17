// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Infrastructure.Options;
public class AzureBlobContentStoreOptions
{
   public required string ConnectionString { get; set; }

   public required string ContainerName { get; set; }
}
