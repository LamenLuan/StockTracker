﻿using Common.Types;
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

      SyncData().Wait();
    }

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

    private new async Task<AppSettings> GetSettings()
    {
      return await _appSettingsCollection.Find(t => true)
        .SingleOrDefaultAsync();
    }

    private new async Task<List<StockTracking>> GetStockTrackingsAsync()
    {
      return await _stockTrackingCollection.Find(t => true)
        .ToListAsync();
    }

    public async Task<bool> CheckDataDifference()
    {
      var appSettings = await GetSettings();
      var localAppSettings = await base.GetSettings();

      if (localAppSettings != null && appSettings.Equals(localAppSettings))
        return true;

      var stockTrackings = await GetStockTrackingsAsync();

      return !stockTrackings.SequenceEqual(StockTrackings);
    }

    #region Auxiliary Methods

    private async Task SyncData()
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

      var stockTrackings = await GetStockTrackingsAsync();

      if (!stockTrackings.SequenceEqual(StockTrackings))
      {
        StockTrackings.RemoveRange(StockTrackings);
        StockTrackings.AddRange(stockTrackings);
      }

      await SaveChangesAsync();
    }

    #endregion
  }
}
