// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Error;

/// <summary>
/// Represents an unhandled application error captured by the site.
/// Maps to db_ErrorLogEntries.
/// </summary>
[PrimaryKey(nameof(Id))]
[Index(nameof(OccurredAtUtc))]
[Index(nameof(TraceIdentifier))]
public sealed class WebsiteError
{
   [Required, Key]
   public int Id { get; set; }

   public required DateTime OccurredAtUtc { get; set; }

   [StringLength(128)]
   public string? TraceIdentifier { get; set; }

   [StringLength(128)]
   public string? CorrelationId { get; set; }

   [StringLength(64)]
   public string? EnvironmentName { get; set; }

   [StringLength(128)]
   public string? ApplicationVersion { get; set; }

   [StringLength(128)]
   public string? MachineName { get; set; }

   [StringLength(16)]
   public string? HttpMethod { get; set; }

   [StringLength(2048)]
   public string? Path { get; set; }

   [StringLength(2048)]
   public string? QueryString { get; set; }

   [StringLength(256)]
   public string? EndpointName { get; set; }

   public string? RouteValuesJson { get; set; }

   public int? StatusCode { get; set; }

   [StringLength(512)]
   public required string ExceptionType { get; set; }

   public required string ExceptionMessage { get; set; }

   [StringLength(512)]
   public string? ExceptionSource { get; set; }

   public string? StackTrace { get; set; }

   [StringLength(512)]
   public string? InnerExceptionType { get; set; }

   public string? InnerExceptionMessage { get; set; }

   [StringLength(256)]
   public string? UserName { get; set; }

   [StringLength(1024)]
   public string? UserAgent { get; set; }

   [StringLength(2048)]
   public string? Referrer { get; set; }

   [StringLength(128)]
   public string? RemoteIpAddress { get; set; }

   public bool IsHandled { get; set; }

   public string? Notes { get; set; }
}
