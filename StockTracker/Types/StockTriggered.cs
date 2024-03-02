namespace StockTracker.Types
{
  public class StockTriggered
  {
    public string Symbol { get; set; } = string.Empty;
    public float Percentage { get; set; }
    public bool TriggeredToBuy { get; set; }

    public override string ToString()
    {
      var action = TriggeredToBuy ? "Buy" : "Sell";
      return $"{action} {Symbol} ({Percentage:#.##}%)";
    }

    public StockTriggered(StockTracking stock, float percentage)
    {
      Symbol = stock.Symbol;
      Percentage = percentage;
      TriggeredToBuy = stock.TrackingToBuy;
    }
  }
}
