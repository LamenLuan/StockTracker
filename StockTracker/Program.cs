using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using StockTracker;
using StockTracker.Types;
using System.Configuration;
using System.Net.NetworkInformation;

internal class Program
{
  private static readonly HttpClient _client = new();
  private static readonly TimeSpan StartTime = new(10, 2, 0);
  private static readonly TimeSpan EndTime = TimeSpan.FromHours(17);
  private static readonly TimeSpan Cooldown = TimeSpan.FromMinutes(30);
  private static readonly List<Tuple<string, float>> StocksTriggered = new();
  private static readonly List<StockTracking> StocksTracked = FileManager.ReadStockTrackings();
  private static readonly string? ApiKey = ConfigurationManager.AppSettings["Brapikey"];

  private static void Main()
  {
    if (!StocksTracked.Any())
    {
      Notify("There was nothing to track. Program closed");
      return;
    }

    InitializeProgram();

    while (true)
    {
      if (DateTime.Now.TimeOfDay >= EndTime) break;

      var connectionTries = 0;
      var apiCommunicated = true;

      for (int i = 0; i < StocksTracked.Count; i++)
      {
        var tracked = StocksTracked[i];
        var uri = new Uri($"https://brapi.dev/api/quote/{tracked.Symbol}?token={ApiKey}");
        string? response;

        try
        {
          response = _client.GetStringAsync(uri).Result;
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

        var stockResults = JsonConvert.DeserializeObject<StocksResults>(response);
        if (stockResults == null) continue;
        if (StockTriggered(stockResults, tracked)) i--;
      }

      NotifyTriggers(StocksTriggered);
      Thread.Sleep(Cooldown);
    }
  }

  private static void InitializeProgram()
  {
    Notify("Program initialized");
    WaitUntilStartTime();
    Notify("Tracker started");
  }

  private static bool StockTriggered(
      StocksResults stocksResults,
      StockTracking tracked
    )
  {
    var priceNow = stocksResults.Results.First().RegularMarketPrice;
    var incomePercentage = ((priceNow / tracked.RegularMarketPrice) - 1) * 100;

    if (incomePercentage >= tracked.TriggerPercentage)
    {
      StocksTriggered.Add(new Tuple<string, float>(tracked.Symbol, incomePercentage));
      StocksTracked.Remove(tracked);
      return true;
    }

    return false;
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
    var timeNow = DateTime.Now.TimeOfDay;

    if (timeNow >= StartTime)
    {
      if (timeNow >= EndTime)
        Thread.Sleep(TimeSpan.FromDays(1) - timeNow + StartTime);
    }
    else Thread.Sleep(StartTime - timeNow);
  }

  private static void NotifyTriggers(List<Tuple<string, float>> stocksTriggered)
  {
    if (stocksTriggered.Any())
    {
      var triggersMessages = stocksTriggered.Select(s => $"{s.Item1}: {s.Item2:#.##}%");
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