using Common;
using Microsoft.AspNetCore.Mvc;
using StockTrackerConfigurator.Models;
using System.Diagnostics;

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

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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