using Common;
using Common.Extensions;
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
      var model = new HomeModel();
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