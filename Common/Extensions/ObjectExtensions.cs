using System.Text.Json;

namespace Common.Extensions
{
  public static class ObjectExtensions
  {
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
      PropertyNameCaseInsensitive = true
    };

    public static string Serialize(this object value)
    {
      return JsonSerializer.Serialize(value, JsonOptions);
    }
  }
}
