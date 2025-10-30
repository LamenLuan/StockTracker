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

      SyncData().Wait();
    }

    private async Task SyncData()
    {
      var appSettings = _appSettingsCollection.Find(t => true).SingleOrDefault();
      var localAppSettings = await GetSettings();

      if (appSettings != null)
        AppSettings.Entry(appSettings).CurrentValues.SetValues(appSettings);
      else
        _ = _appSettingsCollection.InsertOneAsync(localAppSettings);

      var stockTrackings = await _stockTrackingCollection.Find(t => true).ToListAsync();
      StockTrackings.RemoveRange(StockTrackings);
      StockTrackings.AddRange(stockTrackings);

      await SaveChangesAsync();
    }

    public override async Task AddStockTracking(StockTracking stockTracking)
    {
      _ = _stockTrackingCollection.InsertOneAsync(stockTracking);
      await base.AddStockTracking(stockTracking);
    }

    public override async Task RemoveStockTracking(StockTracking stockTracking)
    {
      _ = _stockTrackingCollection.DeleteOneAsync(s => s.Id == stockTracking.Id);
      await base.RemoveStockTracking(stockTracking);
    }

    public override async Task SaveApiKey(string apiKey)
    {
      var appSettings = await GetSettings();
      appSettings.ApiKey = apiKey;
      await SaveChangesAsync();
      _ = _appSettingsCollection.ReplaceOneAsync(s => s.Id == appSettings.Id, appSettings);
    }

    public override async Task SaveTelegramInfo(string botToken, long id)
    {
      var appSettings = await GetSettings();
      appSettings.TelegramBotToken = botToken;
      appSettings.TelegramId = id;
      await SaveChangesAsync();
      _ = _appSettingsCollection.ReplaceOneAsync(s => s.Id == appSettings.Id, appSettings);
    }
  }
}
