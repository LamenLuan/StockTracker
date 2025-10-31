
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
  }
}
