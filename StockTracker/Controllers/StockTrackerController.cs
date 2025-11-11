using Common.DbContexts;
using Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using StockTrackerConfigurator.DTOs;

namespace StockTracker.Controllers
{
  public abstract class StockTrackerController : Controller
  {
    private readonly AppDbContext? _appDbContext;
    protected AppDbContext AppDbContext { get => _appDbContext!; }

    public StockTrackerController(AppDbContext appDbContext)
    {
      var appSettings = appDbContext.GetSettings().GetResult()
        ?? throw new Exception(ReturnDTO.ErrorCannotLoadAppData);

      _appDbContext = appSettings.MongoConnectionString.HasContent()
        ? InsantiateMongoDbContext(appSettings.MongoConnectionString!)
        : appDbContext;
    }

    protected IActionResult ErrorPage(string? message = null)
    {
      return Json(ReturnDTO.Error(message));
    }

    private static MongoDbContext InsantiateMongoDbContext(string connectionString)
    {
      try
      {
        var mongoDbContext = new MongoDbContext(connectionString);
        mongoDbContext.ImportDataFromCloud().Wait();
        return mongoDbContext;
      }
      catch (Exception)
      {
        var msg = "Cannot connect to the clould storage. Please check your connection";
        throw new Exception(msg);
      }
    }
  }
}
