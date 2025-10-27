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
  public class CloudStorageController(AppDbContext appDbContext) : Controller
  {
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<IActionResult> Index()
    {
      var settings = await _appDbContext.GetSettings();
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

      MongoClient client;
      var document = new BsonDocument("ping", 1);

      try
      {
        client = InstantiateMongoClient(dto.MongoConnectionString);
      }
      catch (Exception)
      {
        return Json(ReturnDTO.Error("The informed connection string is invalid"));
      }

      try
      {
        var result = client.GetDatabase("admin").RunCommand<BsonDocument>(document);
      }
      catch (Exception)
      {
        return Json(ReturnDTO.Error("Connection cannot be stablished"));
      }

      var settings = await _appDbContext.GetSettings();
      if (settings == null) return Json(ReturnDTO.Error());

      settings.MongoConnectionString = dto.MongoConnectionString;
      await _appDbContext.SaveChangesAsync();

      return Json(ReturnDTO.Success());
    }
  }
}
