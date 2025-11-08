using Common.DTOs;
using Common.Types;
using MongoDB.Driver;

namespace Common.DbContexts
{
  public class MongoDbContext : AppDbContext
  {
    private readonly IMongoCollection<StockTracking> _stockTrackingCollection;
    private readonly IMongoCollection<AppSettings> _appSettingsCollection;

    public MongoDbContext(string connectionString)
    {
      var settings = MongoClientSettings.FromConnectionString(connectionString);
      settings.ServerApi = new ServerApi(ServerApiVersion.V1);

      var client = new MongoClient(settings);
      var mongoDatabase = client.GetDatabase("StockTracker");

      _stockTrackingCollection = mongoDatabase.GetCollection<StockTracking>(nameof(StockTracking));
      _appSettingsCollection = mongoDatabase.GetCollection<AppSettings>(nameof(AppSettings));
    }

    #region StockTracking

    public override async Task AddStockTracking(StockTracking stockTracking)
    {
      await base.AddStockTracking(stockTracking);
      await _stockTrackingCollection.InsertOneAsync(stockTracking);
    }

    public override async Task RemoveStockTracking(StockTracking stockTracking)
    {
      await base.RemoveStockTracking(stockTracking);
      await _stockTrackingCollection.DeleteOneAsync(s => s.Id == stockTracking.Id);
    }

    private new async Task<List<StockTracking>> GetStockTrackingsAsync()
    {
      return await _stockTrackingCollection.Find(t => true)
        .ToListAsync();
    }

    #endregion

    #region AppSettings

    public override async Task SaveApiKey(string apiKey)
    {
      var appSettings = await base.GetSettings();
      appSettings.ApiKey = apiKey;
      await SaveChangesAsync();
      await _appSettingsCollection.ReplaceOneAsync(s => s.Id == appSettings.Id, appSettings);
    }

    public override async Task SaveTelegramInfo(string botToken, long id)
    {
      var appSettings = await base.GetSettings();
      appSettings.TelegramBotToken = botToken;
      appSettings.TelegramId = id;
      await SaveChangesAsync();
      await _appSettingsCollection.ReplaceOneAsync(s => s.Id == appSettings.Id, appSettings);
    }

    public override async Task SaveMongoConnectionString(string connectionString)
    {
      var appSettings = await base.GetSettings();
      appSettings.MongoConnectionString = connectionString;
      await SaveChangesAsync();
      await _appSettingsCollection.ReplaceOneAsync(s => s.Id == appSettings.Id, appSettings);
    }

    public override async Task SaveSettings(SettingsDTO dto, AppSettings? settings = null)
    {
      settings ??= await base.GetSettings();
      await base.SaveSettings(dto, settings);
      await _appSettingsCollection.ReplaceOneAsync(s => s.Id == settings.Id, settings);
    }

    public async Task UpdateLastNotification(Guid activeTrackerId, DateTime lastNotification)
    {
      var appSettings = await base.GetSettings();
      appSettings.TrackerGuid = activeTrackerId;
      appSettings.LastNotification = lastNotification;

      await _appSettingsCollection.ReplaceOneAsync(s => s.Id == appSettings.Id, appSettings);
    }

    public async Task<(Guid? activeTrackerId, DateTime? lastNotification)> GetLastNotificationInfo()
    {
      var appSettings = await base.GetSettings();
      return (appSettings.TrackerGuid, appSettings.LastNotification);
    }

    private new async Task<AppSettings> GetSettings()
    {
      return await _appSettingsCollection.Find(t => true)
        .SingleOrDefaultAsync();
    }

    #endregion

    public async Task<bool> CheckDataDifference()
    {
      var appSettings = await GetSettings();
      var localAppSettings = await base.GetSettings();

      if (!appSettings.Equals(localAppSettings))
        return true;

      var stockTrackings = await GetStockTrackingsAsync();

      return StockTrackings.Any()
        && !stockTrackings.SequenceEqual(StockTrackings);
    }

    public async Task ImportDataFromCloud()
    {
      var appSettings = await GetSettings();
      var localAppSettings = await base.GetSettings();

      if (appSettings != null)
      {
        if (!appSettings.Equals(localAppSettings))
          AppSettings.Entry(localAppSettings).CurrentValues.SetValues(appSettings);
      }
      else
        await _appSettingsCollection.InsertOneAsync(localAppSettings);

      await OverwriteStockTrackings();

      await SaveChangesAsync();
    }

    public async Task MergeData(bool overwriteLocalData)
    {
      var appSettings = await GetSettings();
      var localAppSettings = await base.GetSettings();

      appSettings.MergeSettings(localAppSettings);

      if (overwriteLocalData)
      {
        AppSettings.Entry(localAppSettings).CurrentValues.SetValues(appSettings);
        await OverwriteStockTrackings();
        await SaveChangesAsync();
        return;
      }

      await _appSettingsCollection.ReplaceOneAsync(f => true, localAppSettings);
      var stockTrackings = await GetStockTrackingsAsync();
      if (!stockTrackings.SequenceEqual(StockTrackings))
      {
        await _stockTrackingCollection.DeleteManyAsync(f => true);
        await _stockTrackingCollection.InsertManyAsync(StockTrackings);
      }
    }

    #region Auxiliary methods

    private async Task OverwriteStockTrackings()
    {
      var stockTrackings = await GetStockTrackingsAsync();

      if (!stockTrackings.SequenceEqual(StockTrackings))
      {
        StockTrackings.RemoveRange(StockTrackings);
        StockTrackings.AddRange(stockTrackings);
      }
    }

    #endregion
  }
}
