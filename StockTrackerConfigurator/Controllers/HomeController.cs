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
      if (resultado == null) return Json(ReturnDTO.Error());
      FileManager.WriteBrapiKey(dto.Key);
      var returnDto = ReturnDTO.Success(dto.Key);
      return Json(returnDto);
    }

    public IActionResult FindStocks(StockSearchDTO dto)
    {
      var token = FileManager.ReadBrapiKey();
      if (!token.HasContent()) return Json(ReturnDTO.Error());

      var url = $"https://brapi.dev/api/available?search={dto.SearchTerm}&token={token}";
      var resultado = _client.GetFromJsonAsync<StockListDTO>(url).Result;
      if (resultado == null) return Json(ReturnDTO.Error());

      return Json(resultado);
    }

    public IActionResult CreateCardView()
    {
      var model = new CreationCardModel();
      return PartialView("_CreationCard", model);
    }

    public IActionResult CreateStockTrack(StockTrackDTO dto)
    {
      var stockTracking = new StockTracking
      {
        RegularMarketPrice = dto.Price,
        TrackingToBuy = dto.Buying,
        TriggerPercentage = dto.TriggerPercentage
      };

      try
      {
        FileManager.WriteNewStockTrack(stockTracking);
        return Json(ReturnDTO.Success());
      }
      catch (Exception)
      {
        return Json(ReturnDTO.Error());
      }
    }

    #region Private methods

    private IActionResult GetBrapiKey(string brapiKey)
    {
      var response = brapiKey.HasContent()
        ? "Some hidden password"
        : string.Empty;

      return Json(response);
    }

    #endregion
  }
}