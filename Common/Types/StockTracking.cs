namespace Common.Types
{
  public class StockTracking : Stock
  {
    public long Id { get; set; }
    public float TriggerPercentage { get; set; }
    public bool TrackingToBuy { get; set; }

    public float PriceTrigger => TrackingToBuy
      ? RegularMarketPrice * (1 - TriggerPercentage / 100)
      : RegularMarketPrice * (1 + TriggerPercentage / 100);
  }
}
