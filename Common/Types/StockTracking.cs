namespace Common.Types
{
  public class StockTracking : Stock
  {
    public long Id { get; set; }
    public float TriggerPercentage { get; set; }
    public bool TrackingToBuy { get; set; }
  }
}
