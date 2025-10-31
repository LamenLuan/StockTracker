using Common.DbContexts;
using Common.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace StockTracker.Controllers
{
  public abstract class StockTrackerController : Controller
  {
    private static AppDbContext? _appDbContext;
    protected static AppDbContext AppDbContext { get => _appDbContext!; }

    public StockTrackerController(AppDbContext appDbContext)
    {
      if (_appDbContext != null) return;
      var appSettings = appDbContext.GetSettings().Result;

      try
      {
        _appDbContext = appSettings.MongoConnectionString.HasContent()
          ? new MongoDbContext(appSettings.MongoConnectionString!)
          : appDbContext;
      }
      catch (Exception)
      {
        var msg = "Cannot connect to the clould storage. Please check your connection";
        throw new Exception(msg);
      }
    }
  }
}
