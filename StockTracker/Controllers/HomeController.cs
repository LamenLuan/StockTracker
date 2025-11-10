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

      var viewModelList = new List<CreationCardViewModel>
      {
        CreationCardViewModel.AddCardButton
      };

      viewModelList.AddRange(stockTrackings.Select(s => new CreationCardViewModel(s)));

      var model = new HomeModel(viewModelList);
      return View(model);
    }

    public async Task<IActionResult> GetBrapiKey()
    {
      var settings = await AppDbContext.GetSettings();
      if (settings == null) return Error();

      return GetBrapiKey(settings.ApiKey);
    }

    public async Task<IActionResult> WriteBrapiKey(BrapiKeyDTO dto)
    {
      var url = $"https://brapi.dev/api/quote/PETR4?token={dto.Key}";
      var resultado = _client.GetStringAsync(url).Result;
      if (string.IsNullOrEmpty(resultado)) return Error();

      await AppDbContext.SaveApiKey(dto.Key);
      var returnDto = ReturnDTO.Success(dto.Key);

      return Json(returnDto);
    }

    public async Task<IActionResult> FindStocks(StockSearchDTO dto)
    {
      var settings = await AppDbContext.GetSettings();
      if (!settings.ApiKey.HasContent()) return Error();

      var url = $"https://brapi.dev/api/available?search={dto.SearchTerm}&token={settings.ApiKey}";
      var resultado = _client.GetFromJsonAsync<StockListDTO>(url).Result;
      if (resultado == null) return Error();

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
        return Success();
      }
      catch (Exception)
      {
        return Error();
      }
    }

    public async Task<IActionResult> RemoveStockTrack(StockTrackDTO dto)
    {
      var stock = await AppDbContext.GetStockTrackingAsync(dto.Id);
      if (stock == null) return Error();
      await AppDbContext.RemoveStockTracking(stock);
      return Success();
    }

    #region Private methods

    private IActionResult GetBrapiKey(string? brapiKey)
    {
      var response = brapiKey.HasContent()
        ? "Some hidden password"
        : string.Empty;

      return Json(response);
    }

    private IActionResult Error() => Json(ReturnDTO.Error());
    private IActionResult Success() => Json(ReturnDTO.Success());

    #endregion
  }
}