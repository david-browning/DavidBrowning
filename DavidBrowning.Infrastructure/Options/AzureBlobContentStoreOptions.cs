// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Infrastructure.Options;

public sealed class AzureBlobContentStoreOptions
{
   /// <summary>
   /// This is either the connection string or the managed identity service
   /// url.
   /// </summary>
   public string? ServiceUri { get; set; }

   public string ContainerName { get; set; } = "content";
}