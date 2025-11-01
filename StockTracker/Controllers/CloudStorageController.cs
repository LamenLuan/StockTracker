using Common.DbContexts;
using Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using StockTracker.DTOs;
using StockTracker.Models;
using StockTrackerConfigurator.DTOs;

namespace StockTracker.Controllers
{
  public class CloudStorageController(AppDbContext appDbContext)
    : StockTrackerController(appDbContext)
  {
    public async Task<IActionResult> Index()
    {
      var settings = await AppDbContext.GetSettings();
      if (settings == null) return Json(ReturnDTO.Error());

      var model = new CloudStorageModel
      {
        HasString = settings.MongoConnectionString.HasContent()
      };

      return View(model);
    }

    public async Task<IActionResult> CheckDataDifference(CloudStorageDTO dto)
    {
      if (string.IsNullOrEmpty(dto.MongoConnectionString))
        return Json(ReturnDTO.Error("No connection string was informed"));

      var localSettings = await AppDbContext.GetSettings();
      if (localSettings == null) return Json(ReturnDTO.Error());

      if (dto.MongoConnectionString.Equals(localSettings.MongoConnectionString))
        return Json(ReturnDTO.Error("The connection string is the same as the current one"));

      if (AppDbContext is MongoDbContext mongoDbContext)
      {
        var dataDifference = await mongoDbContext.CheckDataDifference();
        return Json(ReturnDTO.Success(dataDifference));
      }

      return Json(ReturnDTO.Success(false));
    }

    public async Task<IActionResult> SaveConnectionString(CloudStorageDTO dto)
    {
      if (string.IsNullOrEmpty(dto.MongoConnectionString))
        return Json(ReturnDTO.Error("No connection string was informed"));

      var settings = await AppDbContext.GetSettings();
      if (settings == null) return Json(ReturnDTO.Error());

      if (dto.MongoConnectionString.Equals(settings.MongoConnectionString))
        return Json(ReturnDTO.Error("The connection string is the same as the current one"));

      settings.MongoConnectionString = dto.MongoConnectionString;
      await AppDbContext.SaveChangesAsync();

      return Json(ReturnDTO.Success());
    }
  }
}
