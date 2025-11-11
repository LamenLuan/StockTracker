using Common.DbContexts;
using Common.DTOs;
using Microsoft.AspNetCore.Mvc;
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
      if (settings == null) return ErrorPage(ReturnDTO.ErrorCannotLoadAppData);

      var viewModel = new SettingsDTO
      {
        AppClosingTime = settings.AppClosingTime,
        AppStartTime = settings.AppStartTime,
        Cooldown = settings.Cooldown,
        PriceRange = settings.PriceRange
      };

      var model = new SettingsModel(viewModel);

      return View(model);
    }

    public async Task<IActionResult> SaveSettings(SettingsDTO dto)
    {
      var settings = await AppDbContext.GetSettings();
      if (settings == null) return Json(ReturnDTO.Error());

      if (SettingsDTO.ParseSettingsToDTO(settings).Equals(dto))
        return Json(ReturnDTO.Error("No changes were detected"));

      await AppDbContext.SaveSettings(dto, settings);
      return Json(ReturnDTO.Success());
    }
  }
}
