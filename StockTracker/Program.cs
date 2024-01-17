using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using StockTracker;
using StockTracker.Types;
using System.Configuration;

internal class Program
{
	private static readonly HttpClient _client = new();
	private const int COOLDOWN_MINUTES = 30;

	private static bool TimeToEnd
	{
		get
		{
			var endTime = new TimeSpan(17, 0, 0);
			var timeNow = DateTime.Now.TimeOfDay;
			return (timeNow >= endTime);
		}
	}

	private static void Main()
	{
		var stocksTracked = FileManager.ReadStockTrackings();

		if (stocksTracked == null || !stocksTracked.Any())
		{
			Notify("There was nothing to track. Program closed");
			return;
		}

		Notify("Program started");

		var apiKey = ConfigurationManager.AppSettings["Brapikey"];
		var stocksTriggered = new List<Tuple<string, float>>();

		WaitUntilStartTime();
		Notify("Tracker started");

		while (true)
		{
			if (TimeToEnd) break;

			for (int i = 0; i < stocksTracked.Count; i++)
			{
				var tracked = stocksTracked[i];
				var uri = new Uri($"https://brapi.dev/api/quote/{tracked.Symbol}?token={apiKey}");
				var response = _client.GetStringAsync(uri).Result;
				if (response == null) continue;

				var stock = JsonConvert.DeserializeObject<StocksResults>(response);
				if (stock == null) continue;

				var priceNow = stock.Results.First().RegularMarketPrice;
				var incomePercentage = ((priceNow / tracked.RegularMarketPrice) - 1) * 100;
				if (incomePercentage >= tracked.TriggerPercentage)
				{
					stocksTriggered.Add(new Tuple<string, float>(tracked.Symbol, incomePercentage));
					stocksTracked.Remove(tracked);
					i--;
				}
			}

			if (stocksTriggered.Any())
			{
				var triggersMessages = stocksTriggered.Select(s => $"{s.Item1}: {s.Item2:#.##}%");
				var finalMessage = string.Join("\n", triggersMessages);
				Notify(finalMessage, "Tracker Triggered!");
			}

			Thread.Sleep(COOLDOWN_MINUTES * 60_000);
		}
	}

	private static void WaitUntilStartTime()
	{
		var startTime = new TimeSpan(10, 2, 0);
		var timeNow = DateTime.Now.TimeOfDay;
		if (timeNow >= startTime) return;

		var timeToStart = (int)(startTime - timeNow).TotalMilliseconds;
		Thread.Sleep(timeToStart);
	}

	private static void Notify(string message, string? title = null)
	{
		var toast = new ToastContentBuilder();
		if (title != null) toast.AddText(title);
		toast.AddText(message);
		toast.Show();
	}
}