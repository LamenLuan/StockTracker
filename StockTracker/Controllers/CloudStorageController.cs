using Common.DbContexts;
using Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using StockTracker.Models;
using StockTrackerConfigurator.DTOs;

namespace StockTracker.Controllers
{
  public class CloudStorageController(AppDbContext appDbContext) : Controller
  {
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<IActionResult> IndexAsync()
    {
      var settings = await _appDbContext.GetSettings();
      if (settings == null) return Json(ReturnDTO.Error());

      var model = new CloudStorageModel
      {
        HasString = settings.MongoConnectionString.HasContent()
      };

      return View(model);
    }
  }
}
