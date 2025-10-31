using Common.DbContexts;
using Microsoft.AspNetCore.Mvc;
using StockTracker.DTOs;
using StockTracker.Models;
using StockTrackerConfigurator.DTOs;
using Telegram.Bot;

namespace StockTracker.Controllers
{
  public class TelegramNotifierController(AppDbContext appDbContext)
    : StockTrackerController(appDbContext)
  {
    public async Task<IActionResult> IndexAsync()
    {
      var settings = await AppDbContext.GetSettings();
      if (settings == null) return Json(ReturnDTO.Error());

      var model = new TelegramNotifierModel
      {
        HasToken = settings.HasTelegramConfig()
      };

      return View(model);
    }

    public async Task<IActionResult> SetTelegramNotifier(TelegramNotifierDTO dto)
    {
      if (string.IsNullOrEmpty(dto.Token))
        return Json(ReturnDTO.Error("No token was informed"));

      try
      {
        var client = new TelegramBotClient(dto.Token);
        var returnDto = await SetTelegramInfo(client);
        return Json(returnDto);
      }
      catch (Exception)
      {
        return Json(ReturnDTO.Error("The given token is invalid"));
      }
    }

    public async Task<ReturnDTO> SetTelegramInfo(TelegramBotClient client)
    {
      for (int i = 0; i < 10; i++)
      {
        try
        {
          var updates = await client.GetUpdates();
          if (updates != null && updates.Length > 0)
          {
            var id = updates[0].Message?.Chat.Id;
            if (id.HasValue)
            {
              await AppDbContext.SaveTelegramInfo(client.Token, id.Value);
              return ReturnDTO.Success();
            }
          }
        }
        catch (Exception) { }

        Thread.Sleep(TimeSpan.FromSeconds(1));
      }

      return ReturnDTO.Error("No user message was detected, please try again");
    }
  }
}
