using StockTrackerService.Types;
using Telegram.Bot;

namespace StockTrackerService
{
  public class TelegramNotifier
  {
    private TelegramBotClient Client { get; set; }
    private long Id { get; set; }

    public TelegramNotifier(string token, long id)
    {
      Client = new TelegramBotClient(token);
      Id = id;
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
