using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Types
{
  public class AppSettings
  {
    public long Id { get; set; }
    public string? ApiKey { get; set; }
    public string? TelegramBotToken { get; set; }
    public long? TelegramId { get; set; }
    public string? MongoConnectionString { get; set; }

    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid? TrackerGuid { get; set; }
    public DateTime? LastNotification { get; set; }
    public TimeSpan AppStartTime { get; set; }
    public TimeSpan AppClosingTime { get; set; }
    public float PriceRange { get; set; }
    public TimeSpan Cooldown { get; set; }

    public AppSettings()
    {
      AppStartTime = new TimeSpan(9, 50, 0);
      AppClosingTime = new TimeSpan(17, 0, 0);
      PriceRange = 0.1f;
      Cooldown = new TimeSpan(0, 10, 0);
    }

    public override bool Equals(object? obj)
    {
      return obj is AppSettings settings &&
             Id == settings.Id &&
             ApiKey == settings.ApiKey &&
             TelegramBotToken == settings.TelegramBotToken &&
             TelegramId == settings.TelegramId &&
             MongoConnectionString == settings.MongoConnectionString &&
             EqualityComparer<Guid?>.Default.Equals(TrackerGuid, settings.TrackerGuid) &&
             LastNotification == settings.LastNotification &&
             AppStartTime.Equals(settings.AppStartTime) &&
             AppClosingTime.Equals(settings.AppClosingTime) &&
             PriceRange == settings.PriceRange &&
             Cooldown.Equals(settings.Cooldown);
    }

    public override int GetHashCode()
    {
      HashCode hash = new HashCode();
      hash.Add(Id);
      hash.Add(ApiKey);
      hash.Add(TelegramBotToken);
      hash.Add(TelegramId);
      hash.Add(MongoConnectionString);
      hash.Add(TrackerGuid);
      hash.Add(LastNotification);
      hash.Add(AppStartTime);
      hash.Add(AppClosingTime);
      hash.Add(PriceRange);
      hash.Add(Cooldown);
      return hash.ToHashCode();
    }

    public bool HasTelegramConfig()
    {
      return !string.IsNullOrEmpty(TelegramBotToken) && TelegramId.HasValue;
    }

    public void MergeSettings(AppSettings? other)
    {
      if (other == null) return;

      ApiKey ??= other.ApiKey;
      other.ApiKey ??= ApiKey;

      TelegramBotToken ??= other.TelegramBotToken;
      other.TelegramBotToken ??= TelegramBotToken;

      MongoConnectionString ??= other.MongoConnectionString;
      other.MongoConnectionString ??= MongoConnectionString;

      TelegramId ??= other.TelegramId;
      other.TelegramId ??= TelegramId;
    }
  }
}
