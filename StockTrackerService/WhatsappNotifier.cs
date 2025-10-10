using StockTracker;
using StockTracker.Extensions;
using StockTracker.Types;
using Telegram.Bot;

namespace StockTrackerService
{
  public static class WhatsappNotifier
  {
    private static TelegramBotClient? _client;

    private static TelegramBotClient? Client
    {
      get
      {
        if (_client != null) return _client;

        var token = AppConfigKeys.TOKEN.GetValue();
        if (token == null)
          return null;

        _client = new TelegramBotClient(token);
        return _client;
      }
    }

    public static async Task NotifyStocks(List<StockTriggered> stocks)
    {
      if (stocks == null || stocks.Count == 0 || Client == null) return;

      var id = AppConfigKeys.USER_ID.GetAsLong();

      try
      {
        await Client.SendMessage(id, string.Join("\n", stocks));
      }
      catch (Exception)
      {
        Notifier.Notify("Failed to communicate with Whatsapp API");
      }
    }
  }
}
