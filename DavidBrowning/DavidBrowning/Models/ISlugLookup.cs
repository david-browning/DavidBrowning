// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Models;

/// <summary>
/// A set of properties that many database tables share.
/// Ex: Many tables have an Id field, a slug field, and a display name.
/// Specific class implementations that represent a row in a specific
/// table can inherit this to "share" those common properties.
/// </summary>
public interface ISlugLookup
{
   int Id { get; }

   string Slug { get; }

   string DisplayName { get; }
}
