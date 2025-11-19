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
    private const string MONGO_CONNECTION_ERROR
      = "Cannot connect to the cloud storage. Please check your string and connection";

    public async Task<IActionResult> Index()
    {
      var settings = await AppDbContext.GetSettings();
      if (settings == null) return ErrorPage(ReturnDTO.ErrorCannotLoadAppData);

      var hasString = settings.MongoConnectionString.HasContent();
      var model = new CloudStorageModel(hasString);

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

      var mongoDbContext = CreateMongoDbContext(dto.MongoConnectionString);
      if (mongoDbContext == null)
        return Json(ReturnDTO.Error(MONGO_CONNECTION_ERROR));

      var dataDifference = await mongoDbContext.CheckDataDifference();

      return Json(ReturnDTO.Success(dataDifference));
    }

    public async Task<IActionResult> SaveConnectionString(CloudStorageDTO dto)
    {
      if (string.IsNullOrEmpty(dto.MongoConnectionString))
        return Json(ReturnDTO.Error("No connection string was informed"));

      var settings = await AppDbContext.GetSettings();
      if (settings == null) return Json(ReturnDTO.Error());

      if (dto.MongoConnectionString.Equals(settings.MongoConnectionString))
        return Json(ReturnDTO.Error("The connection string is the same as the current one"));

      if (dto.OverwriteLocalData == null)
      {
        await AppDbContext.SaveMongoConnectionString(dto.MongoConnectionString);
        return Json(ReturnDTO.Success());
      }

      if (AppDbContext is not MongoDbContext mongoDbContext)
      {
        mongoDbContext = CreateMongoDbContext(dto.MongoConnectionString)!;
        if (mongoDbContext == null) return Json(ReturnDTO.Error(MONGO_CONNECTION_ERROR));
      }

      await mongoDbContext.MergeData(dto.OverwriteLocalData.Value);
      await mongoDbContext.SaveMongoConnectionString(dto.MongoConnectionString);

      return Json(ReturnDTO.Success());
    }

    private static MongoDbContext? CreateMongoDbContext(string connectionString)
    {
      try
      {
        return new MongoDbContext(connectionString);
      }
      catch (Exception)
      {
        return null;
      }
    }
  }
}
