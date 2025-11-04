using Common.Types;
using StockTrackerService.Types;
using Telegram.Bot;

namespace StockTrackerService
{
  public class TelegramNotifier
  {
    private TelegramBotClient Client { get; set; }
    private long Id { get; set; }

    public TelegramNotifier(AppSettings appSettings)
    {
      Client = new TelegramBotClient(appSettings.TelegramBotToken!);
      Id = appSettings.TelegramId!.Value;
    }

    public async Task NotifyStocks(List<StockTriggered> stocks)
    {
      if (stocks == null || stocks.Count == 0 || Client == null) return;

      try
      {
        await Client.SendMessage(Id, string.Join("\n", stocks));
      }
      catch (Exception)
      {
        Notifier.Notify("Failed to communicate with Telegram");
      }
    }
  }
}
