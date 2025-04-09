using Common;
using Common.DbContexts;
using Common.Extensions;
using Common.Types;
using StockTracker;
using StockTracker.Extensions;
using StockTracker.Types;
using System.Net.NetworkInformation;

internal class Program
{
  private static TimeSpan EndTime;
  public static TimeSpan Cooldown;
  public static float PriceRange;

  private static List<StockTracking> StocksTracked = new();
  private static AppDbContext _context = null!;
  private static AppSettings _settings = null!;
  private static readonly HttpClient _client = new();
  private static readonly List<StockTriggered> StocksTriggered = new();
  private static readonly List<StockTriggered> StocksNearTrigger = new();

  private static async Task Main()
  {
    if (IsMarketClosedDay()) return;

    using var context = new AppDbContext();
    _context = context;

    if (!await ReadSettings())
    {
      Notifier.Notify("API Key not set, program closed");
      return;
    }

    WaitUntilStartTime();

    while (true)
    {
      if (DateTime.Now.TimeOfDay >= EndTime) break;

      var connectionTries = 0;
      var apiCommunicated = true;
      ReadStockTrackings();

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

      NotifyTriggers();
      Thread.Sleep(Cooldown);
    }
  }

  private static async Task<bool> ReadSettings()
  {
    _settings = await _context.GetSettings();

    EndTime = AppConfigKeys.END_TIME.GetAsTimeSpan();
    PriceRange = AppConfigKeys.NEAR_PRICE_RANGE.GetAsFloat();
    Cooldown = AppConfigKeys.COOLDOWN.GetAsTimeSpan();

    if (string.IsNullOrEmpty(_settings.ApiKey))
      return false;

    return true;
  }

  private static bool ReadStockTrackings()
  {
    StocksTracked = FileManager.ReadStockTrackings();
    return StocksTracked.Any();
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

  private static bool IsMarketClosedDay()
  {
    var day = DateTime.Now.DayOfWeek;
    return day == DayOfWeek.Sunday || day == DayOfWeek.Saturday;
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
    if (StocksTriggered.Any())
      Notifier.NotifyStocks(StocksTriggered, "Tracker Triggered!");

    if (StocksNearTrigger.Any())
      Notifier.NotifyStocks(StocksNearTrigger, "Stocks near triggering!");
  }
}