﻿using System.Text.Json;

namespace StockTracker.Utils.Extensions
{
  public static class StringExtensions
  {
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
      PropertyNameCaseInsensitive = true
    };

    public static T? Deserialize<T>(this string? value)
    {
      if (value == null) return default;
      return JsonSerializer.Deserialize<T>(value, JsonOptions);
    }
  }
}
