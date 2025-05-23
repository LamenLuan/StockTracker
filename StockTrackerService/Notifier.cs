﻿using Microsoft.Toolkit.Uwp.Notifications;
using StockTracker.Types;

namespace StockTracker
{
  public static class Notifier
  {
    public static void NotifyStocks(List<StockTriggered> stocksTrigger, string message)
    {
      var triggersMessages = stocksTrigger.Select(s => s.ToString());
      var finalMessage = string.Join("\n", triggersMessages);
      Notify(finalMessage, message, buttonConfig: ("See stock trackings", "OpenApp"));
      stocksTrigger.Clear();
    }

    public static void Notify(
      string message,
      string? title = null,
      (string title, string argument)? buttonConfig = null)
    {
      var toast = new ToastContentBuilder();
      if (title != null) toast.AddText(title);

      if (buttonConfig.HasValue)
      {
        var (btnTitle, argument) = buttonConfig.Value;
        toast.AddButton(new ToastButton(btnTitle, argument)
        {
          ActivationType = ToastActivationType.Background,
          ActivationOptions = new ToastActivationOptions
          {
            AfterActivationBehavior = ToastAfterActivationBehavior.Default,
          }
        });
      }

      toast.AddText(message);
      toast.Show();
    }
  }
}