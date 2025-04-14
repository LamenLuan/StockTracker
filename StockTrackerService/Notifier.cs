using Microsoft.Toolkit.Uwp.Notifications;
using StockTracker.Types;

namespace StockTracker
{
  public static class Notifier
  {
    public static void NotifyStocks(List<StockTriggered> stocksTrigger, string message)
    {
      var triggersMessages = stocksTrigger.Select(s => s.ToString());
      var finalMessage = string.Join("\n", triggersMessages);
      Notify(finalMessage, message);
      stocksTrigger.Clear();
    }

    public static void Notify(string message, string? title = null, bool openApp = false)
    {
      var toast = new ToastContentBuilder();
      if (title != null) toast.AddText(title);
      toast.AddArgument("OpenApp", openApp);
      toast.AddText(message);
      toast.Show();
    }
  }
}
