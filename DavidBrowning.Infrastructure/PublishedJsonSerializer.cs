using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DavidBrowning.Infrastructure;

public static class PublishedJsonSerializer
{
   public static JsonSerializerOptions Options { get; } = new()
   {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      PropertyNameCaseInsensitive = true,
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
      Converters =
      {
         new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
      },
   };
}
