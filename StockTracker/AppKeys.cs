using System.Configuration;

namespace StockTracker
{
  public static class AppKeys
  {
    public const string BRAPI_KEY = "Brapikey";
    public const string COOLDOWN = "Cooldown";
    public const string START_TIME = "StartTime";
    public const string END_TIME = "EndTime";

    public static string? AppConfigValue(this string key)
      => ConfigurationManager.AppSettings[key];

    public static TimeSpan GetTimeParam(this string key)
    {
      if (!TimeSpan.TryParse(key.AppConfigValue(), out var setting))
        throw new ArgumentOutOfRangeException(nameof(key));

      return setting;
    }
  }
}
