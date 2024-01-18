using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using StockTracker;
using StockTracker.Types;
using System.Configuration;

internal class Program
{
	private const int COOLDOWN_MINUTES = 30;

	private static readonly HttpClient _client = new();
	private static readonly TimeSpan StartTime = new(10, 2, 0);
	private static readonly TimeSpan EndTime = new(17, 0, 0);

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
			if (DateTime.Now.TimeOfDay >= EndTime) break;

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
		var timeNow = DateTime.Now.TimeOfDay;

		if (timeNow >= StartTime)
		{
			if (timeNow >= EndTime)
				Thread.Sleep(TimeSpan.FromDays(1) - timeNow + StartTime);
		}
		else Thread.Sleep(StartTime - timeNow);
	}

	private static void Notify(string message, string? title = null)
	{
		var toast = new ToastContentBuilder();
		if (title != null) toast.AddText(title);
		toast.AddText(message);
		toast.Show();
	}
}