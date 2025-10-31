using Common.DbContexts;
using Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
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

    private static MongoClient InstantiateMongoClient(string connectionString)
    {
      var settings = MongoClientSettings.FromConnectionString(connectionString);
      settings.ServerApi = new ServerApi(ServerApiVersion.V1);

      return new MongoClient(settings);
    }

    public async Task<IActionResult> SaveConnectionString(CloudStorageDTO dto)
    {
      if (string.IsNullOrEmpty(dto.MongoConnectionString))
        return Json(ReturnDTO.Error("No connection string was informed"));

      var document = new BsonDocument("ping", 1);

      try
      {
        var client = InstantiateMongoClient(dto.MongoConnectionString);
        _ = client.GetDatabase("admin").RunCommand<BsonDocument>(document);
      }
      catch (Exception)
      {
        return Json(ReturnDTO.Error("Cannot connect to the clould storage." +
          " Please check connection and the informed credentials"));
      }

      var settings = await AppDbContext.GetSettings();
      if (settings == null) return Json(ReturnDTO.Error());

      settings.MongoConnectionString = dto.MongoConnectionString;
      await AppDbContext.SaveChangesAsync();

      return Json(ReturnDTO.Success());
    }
  }
}
