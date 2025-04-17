using Common.DbContexts;
using Common.Extensions;
using Common.Types;
using Microsoft.Toolkit.Uwp.Notifications;
using StockTracker;
using StockTracker.Extensions;
using StockTracker.Types;
using System.Diagnostics;
using System.Net.NetworkInformation;
using static Common.Constants;

internal class Program
{
  private static TimeSpan EndTime;
  public static TimeSpan Cooldown;
  public static float PriceRange;

  private static List<StockTracking> StocksTracked = [];
  private static AppDbContext _context = null!;
  private static AppSettings _settings = null!;
  private static readonly HttpClient _client = new();
  private static readonly List<StockTriggered> StocksTriggered = [];
  private static readonly List<StockTriggered> StocksNearTrigger = [];

  private static async Task Main()
  {
    if (IsProgramRunningAlready() || IsMarketClosedDay()) return;

    LoadAppResources();
    WaitUntilStartTime();

    while (true)
    {
      if (DateTime.Now.TimeOfDay >= EndTime) break;

      await ReadDbSettings();
      if (IsApiKeyNotSet()) continue;

      await AnalyseTrackings();

      NotifyTriggers();
      Thread.Sleep(Cooldown);
    }
  }

  #region Tracker Business Rules

  private static async Task AnalyseTrackings()
  {
    var connectionTries = 0;
    var apiCommunicated = true;
    await ReadStockTrackingsAsync();

    for (int i = 0; i < StocksTracked.Count; i++)
    {
      var tracked = StocksTracked[i];
      var url = $"https://brapi.dev/api/quote/{tracked.Symbol}?token={_settings.ApiKey}";
      string? response;

      try
      {
        response = _client.GetStringAsync(url).Result;
      }
      catch (Exception)
      {
        if (++connectionTries == 3)
        {
          Notifier.Notify("Failed to communicate with API, program closed");
          return;
        }

        CheckIfApiIsCommunicating(ref apiCommunicated, tracked);
        i--;
        continue;
      }

      connectionTries = 0;
      apiCommunicated = true;
      if (string.IsNullOrEmpty(response)) continue;

      var stockResults = response.Deserialize<StocksResults>();

      if (stockResults == null) continue;
      if (StockTriggered(stockResults, tracked)) i--;
    }
  }

  private static bool StockTriggered(
    StocksResults stocksResults,
    StockTracking tracked
  )
  {
    var priceNow = stocksResults.Results.First().RegularMarketPrice;
    var incomePercentage = ((priceNow / tracked.RegularMarketPrice) - 1) * 100;

    if (PriceTriggered(tracked, incomePercentage))
    {
      var stockTriggered = new StockTriggered(tracked, incomePercentage);
      StocksTriggered.Add(stockTriggered);
      StocksTracked.Remove(tracked);

      return true;
    }

    if (PriceNearTrigger(tracked, priceNow))
    {
      var stockTriggered = new StockTriggered(tracked, incomePercentage);
      StocksNearTrigger.Add(stockTriggered);
    }

    return false;
  }

  private static bool IsMarketClosedDay()
  {
    var day = DateTime.Now.DayOfWeek;
    return day == DayOfWeek.Sunday || day == DayOfWeek.Saturday;
  }

  private static bool PriceNearTrigger(StockTracking tracked, float priceNow)
  {
    var triggerPctg = tracked.TriggerPercentage / 100;
    var desiredPricePctg = 1 + (tracked.TrackingToBuy ? -triggerPctg : triggerPctg);
    var desiredPrice = tracked.RegularMarketPrice * desiredPricePctg;
    var rangeValue = desiredPrice * PriceRange / 100;

    return priceNow >= desiredPrice - rangeValue && priceNow <= desiredPrice + rangeValue;
  }

  private static bool PriceTriggered(StockTracking tracked, float percentage)
  {
    return TriggeredToBuy(tracked, -percentage)
      || TriggeredToSell(tracked, percentage);
  }

  private static bool TriggeredToBuy(StockTracking tracked, float percentage)
    => tracked.TrackingToBuy && percentage >= tracked.TriggerPercentage;

  private static bool TriggeredToSell(StockTracking tracked, float percentage)
  => !tracked.TrackingToBuy && percentage >= tracked.TriggerPercentage;

  #endregion

  private static bool IsApiKeyNotSet()
  {
    if (string.IsNullOrEmpty(_settings.ApiKey))
    {
      Notifier.Notify("API Key not set", buttonConfig: ("Configure Key", "OpenApp"));
      Thread.Sleep(Cooldown);
      return true;
    }

    return false;
  }

  private static void LoadAppResources()
  {
    ReadAppSettings();
    AddToastClickEvent();
    _context = new AppDbContext();
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

  private static async Task<bool> ReadDbSettings()
  {
    _settings = await _context.GetSettings();

    if (string.IsNullOrEmpty(_settings.ApiKey))
      return false;

    return true;
  }

  private static void ReadAppSettings()
  {
    EndTime = AppConfigKeys.END_TIME.GetAsTimeSpan();
    PriceRange = AppConfigKeys.NEAR_PRICE_RANGE.GetAsFloat();
    Cooldown = AppConfigKeys.COOLDOWN.GetAsTimeSpan();
  }

  private static async Task<bool> ReadStockTrackingsAsync()
  {
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
    var startTime = AppConfigKeys.START_TIME.GetAsTimeSpan();
    var timeNow = DateTime.Now.TimeOfDay;

    if (timeNow >= startTime)
    {
      if (timeNow >= EndTime)
        Thread.Sleep(TimeSpan.FromDays(1) - timeNow + startTime);
    }
    else Thread.Sleep(startTime - timeNow);

    Notifier.Notify("Tracker started");
  }

  private static void NotifyTriggers()
  {
    if (StocksTriggered.Count != 0)
      Notifier.NotifyStocks(StocksTriggered, "Tracker Triggered!");

    if (StocksNearTrigger.Count != 0)
      Notifier.NotifyStocks(StocksNearTrigger, "Stocks near triggering!");
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
}