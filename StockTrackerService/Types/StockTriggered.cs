using Common.Types;

namespace StockTracker.Types
{
  public class StockTriggered
  {
    public string Symbol { get; set; } = string.Empty;
    public float Price { get; set; }
    public float CurrentPrice { get; set; }
    public bool TriggeredToBuy { get; set; }

    public override string ToString()
    {
      var action = TriggeredToBuy ? "Buy" : "Sell";
      var msg = $"{action} {Symbol} for R$ {Price:0.00}";
      if (CurrentPrice != Price) msg += $" (It's R$ {CurrentPrice:0.00} now)";
      return msg;
    }

    public StockTriggered(StockTracking stock, float currentPrice)
    {
      Symbol = stock.Symbol;
      CurrentPrice = currentPrice;
      TriggeredToBuy = stock.TrackingToBuy;
      Price = stock.RegularMarketPrice;
    }
  }
}
