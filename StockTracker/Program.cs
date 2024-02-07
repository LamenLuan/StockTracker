using Microsoft.Toolkit.Uwp.Notifications;
using StockTracker;
using StockTracker.Types;
using System.Net.NetworkInformation;

internal class Program
{
  private static readonly TimeSpan StartTime = new(10, 2, 0);
  private static readonly TimeSpan EndTime = TimeSpan.FromHours(17);
  private static readonly TimeSpan Cooldown = TimeSpan.FromMinutes(30);
  private static readonly List<Tuple<string, float>> StocksTriggered = new();
  private static readonly List<StockTracking> StocksTracked = FileManager.ReadStockTrackings();

  private static void Main()
  {
    if (!StocksTracked.Any())
    {
      Notify("There was nothing to track. Program closed");
      return;
    }

    WaitUntilStartTime();

    while (true)
    {
      if (DateTime.Now.TimeOfDay >= EndTime) break;

      var connectionTries = 0;
      var apiCommunicated = true;

      for (int i = 0; i < StocksTracked.Count; i++)
      {
        var tracked = StocksTracked[i];
        Stock? stock;

        try
        {
          stock = IbovScrapper.FindStockInfos(tracked.Symbol);
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

        if (StockTriggered(stock, tracked)) i--;
      }

      NotifyTriggers(StocksTriggered);
      Thread.Sleep(Cooldown);
    }
  }

  private static bool StockTriggered(Stock? stock, StockTracking tracked)
  {
    if (stock == null) return false;

    var incomePercentage = ((stock.Price / tracked.Price) - 1) * 100;

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
      return myPing.Send("google.com/finance").Status == IPStatus.Success;
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