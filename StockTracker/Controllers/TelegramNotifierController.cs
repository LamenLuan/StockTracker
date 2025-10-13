using Common.DbContexts;
using Microsoft.AspNetCore.Mvc;
using StockTracker.Models;
using StockTracker.ViewModels;
using StockTrackerConfigurator.DTOs;

namespace StockTracker.Controllers
{
  public class TelegramNotifierController(AppDbContext appDbContext) : Controller
  {
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<IActionResult> IndexAsync()
    {
      var settings = await _appDbContext.GetSettings();
      if (settings == null) return Json(ReturnDTO.Error());

      var viewModel = new TelegramNotifierViewModel
      {
        Token = settings.TelegramBotToken,
        Id = settings.TelegramId
      };

      var model = new TelegramNotifierModel(viewModel);
      return View(model);
    }
  }
}
