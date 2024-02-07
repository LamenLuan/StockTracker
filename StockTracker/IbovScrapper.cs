using HtmlAgilityPack;
using StockTracker.Types;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StockTracker
{
  public class IbovScrapper
  {
    private const string BASE_URL = "https://www.google.com/finance/quote/";
    private const string REGEX_PRICE_PATTERN = @"\d+(\.\d{1,2})?";
    private const string REGEX_CURRENCY_PATTERN = @"R\$" + REGEX_PRICE_PATTERN;

    private static readonly HttpClient HttpClient = new();

    public static Stock? FindStockInfos(string stockCode)
    {
      var uri = new Uri($"{BASE_URL}{stockCode}:BVMF");
      var html = HttpClient.GetStringAsync(uri).Result;
      var htmlDocument = new HtmlDocument();
      htmlDocument.LoadHtml(html);

      var div = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'fxKbKc')]");
      var value = div.InnerText;

      if (!Regex.IsMatch(value, REGEX_CURRENCY_PATTERN)) return null;

      var priceStr = Regex.Match(value, REGEX_PRICE_PATTERN).Value;
      var price = float.Parse(priceStr, CultureInfo.InvariantCulture);

      return new Stock
      {
        Symbol = stockCode,
        RegularMarketPrice = price
      };
    }
  }
}
