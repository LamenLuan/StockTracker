using HtmlAgilityPack;
using StockTracker.Types;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StockTracker
{
  public class IbovScrapper
  {
    private const string BASE_URL = "https://www.google.com/finance/quote/";
    private const string REGEX_PRICE_PATTERN = @"\d+\.\d{2}";
    private const string X_PATH = "//div[contains(@class, 'fxKbKc')]";

    private static readonly HttpClient HttpClient = new();

    public static Stock? FindStockInfos(string stockCode)
    {
      var uri = new Uri($"{BASE_URL}{stockCode}:BVMF");
      var html = HttpClient.GetStringAsync(uri).Result;
      HtmlDocument htmlDocument = new();
      htmlDocument.LoadHtml(html);

      var div = htmlDocument.DocumentNode.SelectSingleNode(X_PATH);
      var value = div.InnerText;

      var match = Regex.Match(value, REGEX_PRICE_PATTERN);
      if (!match.Success) return null;

      var price = float.Parse(match.Value, CultureInfo.InvariantCulture);

      return new Stock
      {
        Symbol = stockCode,
        Price = price
      };
    }
  }
}
