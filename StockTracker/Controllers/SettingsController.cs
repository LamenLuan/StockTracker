using Common.DbContexts;
using Microsoft.AspNetCore.Mvc;
using StockTracker.DTOs;
using StockTracker.Models;
using StockTrackerConfigurator.DTOs;

namespace StockTracker.Controllers
{
  public class SettingsController(AppDbContext appDbContext)
    : StockTrackerController(appDbContext)
  {
    public async Task<IActionResult> Index()
    {
      var settings = await AppDbContext.GetSettings();
      if (settings == null) return Json(ReturnDTO.Error());

      var model = new CloudStorageModel
      {
        Settings = new SettingsDTO
        {
          AppClosingTime = settings.AppClosingTime,
          AppStartTime = settings.AppStartTime,
          Cooldown = settings.Cooldown,
          PriceRange = settings.PriceRange
        }
      };

      return View(model);
    }
  }
}
