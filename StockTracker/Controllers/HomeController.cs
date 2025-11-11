using Common.DbContexts;
using Common.Extensions;
using Common.Types;
using Microsoft.AspNetCore.Mvc;
using StockTracker.Models;
using StockTracker.ViewModels;
using StockTrackerConfigurator.DTOs;
using StockTrackerConfigurator.Models;

namespace StockTracker.Controllers
{
  public class HomeController(AppDbContext appDbContext)
    : StockTrackerController(appDbContext)
  {
    private static readonly HttpClient _client = new();

    public async Task<IActionResult> IndexAsync()
    {
      var stockTrackings = await AppDbContext.GetStockTrackingsAsync();
      var settings = await AppDbContext.GetSettings();

      if (stockTrackings == null || settings == null)
        return ErrorPage(ReturnDTO.ErrorCannotLoadAppData);

      var viewModelList = new List<CreationCardViewModel>
      {
        CreationCardViewModel.AddCardButton
      };

      viewModelList.AddRange(stockTrackings.Select(s => new CreationCardViewModel(s)));
      var hasKey = settings.ApiKey.HasContent();

      var model = new HomeModel(viewModelList, hasKey);
      return View(model);
    }

    public async Task<IActionResult> WriteBrapiKey(BrapiKeyDTO dto)
    {
      var url = $"https://brapi.dev/api/quote/GOGL34?token={dto.Key}";
      var resultado = await _client.GetStringAsync(url).GetResultAsync();

      if (string.IsNullOrEmpty(resultado))
        return Json(ReturnDTO.Error("Please verify your API key and internet connection"));

      await AppDbContext.SaveApiKey(dto.Key);
      var returnDto = ReturnDTO.Success(dto.Key);

      return Json(returnDto);
    }

    public async Task<IActionResult> FindStocks(StockSearchDTO dto)
    {
      var settings = await AppDbContext.GetSettings();
      if (settings == null || !settings.ApiKey.HasContent())
        return Json(ReturnDTO.Error());

      var url = $"https://brapi.dev/api/available?search={dto.SearchTerm}";
      var resultado = await _client.GetFromJsonAsync<StockListDTO>(url).GetResultAsync();
      if (resultado == null) return Json(ReturnDTO.Error());

      return Json(resultado);
    }

    public IActionResult CreateCardView()
    {
      var model = new CreationCardModel(CreationCardViewModel.CardForm);
      return PartialView("_FormCreationCard", model);
    }

    public async Task<IActionResult> CreateStockTrack(StockTrackDTO dto)
    {
      var stockTracking = new StockTracking
      {
        RegularMarketPrice = dto.Price,
        TrackingToBuy = dto.Buying,
        TriggerPercentage = dto.TriggerPercentage,
        Symbol = dto.StockName
      };

      try
      {
        await AppDbContext.AddStockTracking(stockTracking);
        return Json(ReturnDTO.Success());
      }
      catch (Exception)
      {
        return Json(ReturnDTO.Error());
      }
    }

    public async Task<IActionResult> RemoveStockTrack(StockTrackDTO dto)
    {
      var stock = await AppDbContext.GetStockTrackingAsync(dto.Id);
      if (stock == null) return Json(ReturnDTO.Error());

      await AppDbContext.RemoveStockTracking(stock);

      return Json(ReturnDTO.Success());
    }
  }
}