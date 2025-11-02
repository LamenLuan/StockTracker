
using Common.Extensions;

namespace Common.Types
{
  public class AppSettings
  {
    public long Id { get; set; }
    public string? ApiKey { get; set; }
    public string? TelegramBotToken { get; set; }
    public long? TelegramId { get; set; }
    public string? MongoConnectionString { get; set; }

    public override bool Equals(object? obj)
    {
      return obj is AppSettings settings &&
             Id == settings.Id &&
             ApiKey == settings.ApiKey &&
             TelegramBotToken == settings.TelegramBotToken &&
             TelegramId == settings.TelegramId &&
             MongoConnectionString == settings.MongoConnectionString;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Id, ApiKey, TelegramBotToken, TelegramId, MongoConnectionString);
    }

    public bool HasTelegramConfig()
    {
      return !string.IsNullOrEmpty(TelegramBotToken) && TelegramId.HasValue;
    }

    public bool HasConflictingSettings(AppSettings? other)
    {
      if (other == null) return false;

      return
        HasContentConflict(ApiKey, other.ApiKey)
        || HasContentConflict(TelegramBotToken, other.TelegramBotToken)
        || HasContentConflict(MongoConnectionString, other.MongoConnectionString);
    }

    #region Auxiliary Methods

    private static bool HasContentConflict(string? value, string? otherValue)
      => value.HasContent() && otherValue.HasContent() && !value!.Equals(otherValue);

    #endregion
  }
}
