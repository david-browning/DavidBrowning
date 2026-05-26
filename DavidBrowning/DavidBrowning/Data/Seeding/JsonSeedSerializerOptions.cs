// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DavidBrowning.Data.Seeding;

public static class JsonSeedSerializerOptions
{
   public static JsonSerializerOptions CreateDefault()
   {
      JsonSerializerOptions options = new()
      {
         PropertyNameCaseInsensitive = false,
         ReadCommentHandling = JsonCommentHandling.Skip,
         AllowTrailingCommas = true
      };

      options.Converters.Add(new JsonStringEnumConverter());
      options.Converters.Add(new DateOnlyJsonConverter());

      return options;
   }

   private sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
   {
      private const string _dateFormat = "yyyy-MM-dd";

      public override DateOnly Read(
          ref Utf8JsonReader reader,
          Type typeToConvert,
          JsonSerializerOptions options)
      {
         string? value = reader.GetString();
         if (string.IsNullOrWhiteSpace(value))
         {
            throw new JsonException("DateOnly seed value cannot be null or empty.");
         }

         return DateOnly.ParseExact(value, _dateFormat, CultureInfo.InvariantCulture);
      }

      public override void Write(
          Utf8JsonWriter writer,
          DateOnly value,
          JsonSerializerOptions options)
      {
         writer.WriteStringValue(value.ToString(_dateFormat, CultureInfo.InvariantCulture));
      }
   }
}
