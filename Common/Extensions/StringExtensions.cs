using System.Text.Json;

namespace Common.Extensions
{
  public static class StringExtensions
  {
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
      PropertyNameCaseInsensitive = true
    };

    public static string Serialize<T>(this T value)
    {
      return JsonSerializer.Serialize(value, JsonOptions);
    }

    public static T? Deserialize<T>(this string? value)
    {
      if (value == null) return default;
      return JsonSerializer.Deserialize<T>(value, JsonOptions);
    }

    public static bool HasContent(this string? value)
      => !string.IsNullOrEmpty(value);
  }
}
