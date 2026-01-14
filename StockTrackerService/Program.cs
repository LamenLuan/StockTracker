using Common.DbContexts;
using Common.Extensions;
using Common.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.Notifications;
using StockTrackerService;
using StockTrackerService.Types;
using System.Diagnostics;
using System.Net.NetworkInformation;
using static Common.Constants;

internal class Program
{
  private static Guid Guid { get; set; } = Guid.NewGuid();
  private static List<StockTracking> StocksTracked = [];
  private static AppDbContext? _context = null!;
  private static TelegramNotifier _telegramNotifier = null!;
  private static AppSettings _settings = null!;

  private static readonly HttpClient _client = new();
  private static readonly List<StockTriggered> StocksTriggered = [];
  private static readonly List<StockTriggered> StocksNearTrigger = [];

  private static async Task Main()
  {
    if (CantRunTracker()) return;

    await LoadAppResources();
    Notifier.Notify("Program initialized");

    while (true)
    {
      WaitUntilStartTime();

      if (IsApiKeySet())
      {
        await AnalyseTrackings();
        await NotifyTriggersAsync();
      }

      _context?.Dispose();
      _context = null;
      await WaitCooldownLoadDbContext();
    }
  }

  #region Tracker Business Rules

  private static async Task AnalyseTrackings()
  {
    var connectionTries = 0;
    var apiCommunicated = true;
    await ReadStockTrackingsAsync();
    var trackingsToNotify = StocksTracked.Where(s => !s.NotificationMuted).ToList();

    for (int i = 0; i < trackingsToNotify.Count; i++)
    {
      var tracked = trackingsToNotify[i];
      var url = $"https://brapi.dev/api/quote/{tracked.Symbol}?token={_settings.ApiKey}";
      string? response;

      try
      {
        response = await _client.GetStringAsync(url);
      }
      catch (Exception)
      {
        if (++connectionTries == 3) return;
        CheckIfApiIsCommunicating(ref apiCommunicated, tracked);
        i--;
        continue;
      }

      connectionTries = 0;
      apiCommunicated = true;
      if (string.IsNullOrEmpty(response)) continue;

      var stockResults = response.Deserialize<StocksResults>();
      if (stockResults == null) continue;

      CheckIfStockTriggered(stockResults, tracked);
    }
  }

  private static void CheckIfStockTriggered(
    StocksResults stocksResults,
    StockTracking tracked
  )
  {
    var priceNow = stocksResults.Results.First().RegularMarketPrice;
    if (priceNow.Equals(0f)) return;

    if (PriceTriggered(tracked, priceNow))
    {
      var stockTriggered = new StockTriggered(tracked, priceNow);
      StocksTriggered.Add(stockTriggered);
      return;
    }

    if (PriceTriggered(tracked, priceNow, _settings.PriceRange))
    {
      var stockTriggered = new StockTriggered(tracked, priceNow);
      StocksNearTrigger.Add(stockTriggered);
    }
  }

  private static bool IsMarketClosedDay()
  {
    var day = DateTime.Now.DayOfWeek;
    return day == DayOfWeek.Sunday || day == DayOfWeek.Saturday;
  }

  private static bool PriceTriggered(
    StockTracking tracked,
    float priceNow,
    float priceRange = 0f)
  {
    var desiredPrice = tracked.PriceTrigger;
    var rangeValue = desiredPrice * priceRange / 100f;

    return tracked.TrackingToBuy
      ? priceNow <= desiredPrice + rangeValue
      : priceNow >= desiredPrice - rangeValue;
  }

  #endregion

  private static bool CantRunTracker()
  {
    return !Debugger.IsAttached &&
      (IsProgramRunningAlready() || IsMarketClosedDay());
  }

  private static bool IsApiKeySet()
  {
    if (string.IsNullOrEmpty(_settings.ApiKey))
    {
      Notifier.Notify("API Key not set", buttonConfig: ("Configure Key", "OpenApp"));
      return false;
    }

    return true;
  }

  private static async Task LoadAppResources()
  {
    AddToastClickEvent();
    await LoadDbContext();
    if (_settings.HasTelegramConfig())
      _telegramNotifier = new TelegramNotifier(_settings);
  }

  private static async Task LoadDbContext()
  {
    if (_context == null)
    {
      _context = new AppDbContext();
      _context.Database.Migrate();
      _settings = await _context.GetSettings();
    }

    if (_settings.MongoConnectionString.HasContent())
      _context = await CreateMongoDbContext();

    _settings = await _context.GetSettings();
  }

  private static async Task<MongoDbContext> CreateMongoDbContext()
  {
    try
    {
      var mongoDbContext = new MongoDbContext(_settings.MongoConnectionString!);
      await mongoDbContext.ImportDataFromCloud();
      return mongoDbContext;
    }
    catch (Exception)
    {
      Notifier.Notify("Cannot connect to the clould storage. Program closed");
      throw;
    }
  }

  private static bool IsProgramRunningAlready()
  {
    _ = new Mutex(true, SERVICE_MUTEX, out bool createdNew);

    if (!createdNew)
    {
      Notifier.Notify("Program already running");
      return true;
    }
    return false;
  }

  private static async Task<bool> ReadStockTrackingsAsync()
  {
    if (_context == null) return false;
    StocksTracked = await _context.GetStockTrackingsAsync();
    return StocksTracked.Count != 0;
  }

  private static void CheckIfApiIsCommunicating(
      ref bool apiCommunicated, StockTracking tracked
    )
  {
    if (ApiIsCommunicating())
    {
      if (apiCommunicated) StocksTracked.Remove(tracked);
      apiCommunicated = true;
    }
    else
    {
      Thread.Sleep(TimeSpan.FromSeconds(2));
      apiCommunicated = false;
    }
  }

  private static bool ApiIsCommunicating()
  {
    try
    {
      Ping myPing = new();
      return myPing.Send("brapi.dev").Status == IPStatus.Success;
    }
    catch (Exception)
    {
      return false;
    }
  }

  private static void WaitUntilStartTime()
  {
    if (Debugger.IsAttached) return;

    var timeNow = DateTime.Now.TimeOfDay;

    if (timeNow < _settings.AppStartTime)
    {
      Thread.Sleep(_settings.AppStartTime - timeNow);
    }
    else if (timeNow >= _settings.AppClosingTime)
    {
      var sleepTime = TimeSpan.FromDays(1) - timeNow + _settings.AppStartTime;
      Thread.Sleep(sleepTime);
    }
  }

  private static async Task NotifyTriggersAsync()
  {
    var allTriggers = StocksTriggered.Concat(StocksNearTrigger).ToList();
    if (allTriggers.Count == 0) return;

    Notifier.NotifyStocks(StocksTriggered, StocksNearTrigger);

    if (_telegramNotifier != null)
    {
      if (_context is MongoDbContext mongoDbContext)
      {
        var cooldown = _settings.Cooldown + TimeSpan.FromMinutes(1);
        var (activeTrackerId, lastNotification) = await mongoDbContext.GetLastNotificationInfo();

        TimeSpan? timeSinceLastNotification = lastNotification.HasValue
          ? DateTime.UtcNow - lastNotification.Value
          : null;

        if (Guid != activeTrackerId && timeSinceLastNotification.HasValue && cooldown > timeSinceLastNotification)
          return;

        await mongoDbContext.UpdateLastNotification(Guid, DateTime.UtcNow);
      }

      await _telegramNotifier.NotifyStocks(allTriggers);
    }

    StocksTriggered.Clear();
    StocksNearTrigger.Clear();
  }

  private static void AddToastClickEvent()
  {
    ToastNotificationManagerCompat.OnActivated += toastArgs =>
    {
      var args = ToastArguments.Parse(toastArgs.Argument);
      var openAppArg = args.Where(a => a.Key == "OpenApp").SingleOrDefault();
      if (openAppArg.Key != null)
        OpenFrontApp();
    };
  }

  private static void OpenFrontApp()
  {
    if (Mutex.TryOpenExisting(APP_MUTEX, out _))
    {
      Process.Start(new ProcessStartInfo
      {
        FileName = APP_URL,
        UseShellExecute = true
      });

      return;
    }

    var exePath = $"{AppDomain.CurrentDomain.BaseDirectory}{APP_NAME}.exe";

    try
    {
      Process.Start(new ProcessStartInfo { FileName = exePath });
    }
    catch (Exception)
    {
      Notifier.Notify("Could not open the application");
    }
  }

  private static async Task WaitCooldownLoadDbContext()
  {
    var totalTimeWaited = TimeSpan.Zero;

    while (true)
    {
      var coolDown = _settings.Cooldown;
      totalTimeWaited += coolDown;

      Thread.Sleep(coolDown);

      await LoadDbContext();
      if (totalTimeWaited >= _settings.Cooldown) break;
    }
  }
}