using Common;
using Common.Extensions;
using Common.Types;
using Microsoft.AspNetCore.Mvc;
using StockTrackerConfigurator.DTOs;
using StockTrackerConfigurator.Models;

namespace StockTrackerConfigurator.Controllers
{
  public class HomeController : Controller
  {
    private static readonly HttpClient _client = new();

    public IActionResult Index()
    {
      var stockTrackings = FileManager.ReadStockTrackings();

      var viewModelList = new List<CreationCardViewModel> { CreationCardViewModel.AddCardButton };
      viewModelList.AddRange(stockTrackings.Select(s => new CreationCardViewModel(s)));

      var model = new HomeModel(viewModelList);
      return View(model: model);
    }

    public IActionResult GetBrapiKey()
    {
      var brapiKey = FileManager.ReadBrapiKey();
      return GetBrapiKey(brapiKey);
    }

    public IActionResult CheckIfBrapiKeyValid(BrapiKeyDTO dto)
    {
      var url = $"https://brapi.dev/api/quote/PETR4?token={dto.Key}";
      var resultado = _client.GetStringAsync(url).Result;
      if (resultado == null) return Error();
      FileManager.WriteBrapiKey(dto.Key);
      var returnDto = ReturnDTO.Success(dto.Key);
      return Json(returnDto);
    }

    public IActionResult FindStocks(StockSearchDTO dto)
    {
      var token = FileManager.ReadBrapiKey();
      if (!token.HasContent()) return Error();

      var url = $"https://brapi.dev/api/available?search={dto.SearchTerm}&token={token}";
      var resultado = _client.GetFromJsonAsync<StockListDTO>(url).Result;
      if (resultado == null) return Error();

      return Json(resultado);
    }

    public IActionResult CreateCardView()
    {
      var model = new CreationCardModel(CreationCardViewModel.CardForm);
      return PartialView("_FormCreationCard", model);
    }

    public IActionResult CreateStockTrack(StockTrackDTO dto)
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
        FileManager.WriteNewStockTrack(stockTracking);
        return Success();
      }
      catch (Exception)
      {
        return Error();
      }
    }

    public IActionResult RemoveStockTrack(StockTrackDTO dto)
    {
      var stocks = FileManager.ReadStockTrackings();
      var idx = stocks.FindIndex(s => s.Symbol.Equals(dto.StockName) && s.TrackingToBuy == dto.Buying);
      if (idx < 0) return Error();
      stocks.RemoveAt(idx);
      return FileManager.WriteStockTrackings(stocks) ? Success() : Error();
    }

    #region Private methods

    private IActionResult GetBrapiKey(string brapiKey)
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