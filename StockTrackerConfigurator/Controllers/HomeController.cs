using Common;
using Microsoft.AspNetCore.Mvc;
using StockTrackerConfigurator.Models;

namespace StockTrackerConfigurator.Controllers
{
	public class HomeController : Controller
	{
		private static readonly HttpClient _client = new();

		public IActionResult Index()
		{
			var brapiKey = FileManager.ReadBrapiKey();
			var model = new HomeModel();
			return View(model: model);
		}

		private bool BrapiKeyValid(string brapiKey)
		{
			var url = $"https://brapi.dev/api/quote/PETR4?token={brapiKey}";
			var resultado = _client.GetStringAsync(url).Result;
			if (resultado == null) return false;
			return true;
		}
	}
}