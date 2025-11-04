
using Common.Extensions;
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

    public override bool Equals(object? obj)
    {
      return obj is AppSettings settings &&
             Id == settings.Id &&
             ApiKey == settings.ApiKey &&
             TelegramBotToken == settings.TelegramBotToken &&
             TelegramId == settings.TelegramId &&
             MongoConnectionString == settings.MongoConnectionString &&
             EqualityComparer<Guid?>.Default.Equals(TrackerGuid, settings.TrackerGuid) &&
             LastNotification == settings.LastNotification;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Id, ApiKey, TelegramBotToken, TelegramId, MongoConnectionString, TrackerGuid, LastNotification);
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

    #region Auxiliary Methods

    private static bool HasContentConflict(string? value, string? otherValue)
      => value.HasContent() && otherValue.HasContent() && !value!.Equals(otherValue);

    #endregion
  }
}
