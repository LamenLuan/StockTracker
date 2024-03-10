using Microsoft.Toolkit.Uwp.Notifications;
using StockTracker;
using StockTracker.Types;
using StockTracker.Utils.Extensions;
using System.Net.NetworkInformation;

internal class Program
{
  private static TimeSpan Cooldown;
  private static TimeSpan StartTime;
  private static TimeSpan EndTime;
  private static List<StockTracking> StocksTracked = new();

  private static readonly HttpClient _client = new();
  private static readonly List<StockTriggered> StocksTriggered = new();
  private static readonly string? ApiKey = AppKeys.BRAPI_KEY.AppConfigValue();

  private static void Main()
  {
    ReadSettings();
    WaitUntilStartTime();

    while (true)
    {
      if (DateTime.Now.TimeOfDay >= EndTime) break;

      var connectionTries = 0;
      var apiCommunicated = true;

      for (int i = 0; i < StocksTracked.Count; i++)
      {
        var tracked = StocksTracked[i];
        var url = $"https://brapi.dev/api/quote/{tracked.Symbol}?token={ApiKey}";
        string? response;

        try
        {
          response = _client.GetStringAsync(url).Result;
        }
        catch (Exception)
        {
          if (++connectionTries == 3)
          {
            Notify("Failed to communicate with API, program closed");
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

      NotifyTriggers(StocksTriggered);
      Thread.Sleep(Cooldown);
      ReadStockTrackings();
    }
  }

  private static void ReadSettings()
  {
    ReadStockTrackings();

    try
    {
      Cooldown = AppKeys.COOLDOWN.GetTimeParam();
      StartTime = AppKeys.START_TIME.GetTimeParam();
      EndTime = AppKeys.END_TIME.GetTimeParam();
    }
    catch (ArgumentOutOfRangeException ex)
    {
      Notify($"Program failed to load the setting [{ex.ParamName}]");
      throw;
    }
  }

  private static void ReadStockTrackings()
  {
    StocksTracked = FileManager.ReadStockTrackings();

    if (!StocksTracked.Any())
    {
      Notify("No tracker found in data file. Program closed");
      throw new ArgumentOutOfRangeException(nameof(StocksTracked));
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

    return false;
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

  private static void WaitUntilStartTime()
  {
    Notify("Program initialized");
    var timeNow = DateTime.Now.TimeOfDay;

    if (timeNow >= StartTime)
    {
      if (timeNow >= EndTime)
        Thread.Sleep(TimeSpan.FromDays(1) - timeNow + StartTime);
    }
    else Thread.Sleep(StartTime - timeNow);

    Notify("Tracker started");
  }

  private static void NotifyTriggers(List<StockTriggered> stocksTriggered)
  {
    if (stocksTriggered.Any())
    {
      var triggersMessages = stocksTriggered.Select(s => s.ToString());
      var finalMessage = string.Join("\n", triggersMessages);
      Notify(finalMessage, "Tracker Triggered!");
    }
  }

  private static void Notify(string message, string? title = null)
  {
    var toast = new ToastContentBuilder();
    if (title != null) toast.AddText(title);
    toast.AddText(message);
    toast.Show();
  }
}