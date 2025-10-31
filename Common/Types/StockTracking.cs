using MongoDB.Bson.Serialization.Attributes;

namespace Common.Types
{
  public class StockTracking : Stock
  {
    [BsonId]
    public long Id { get; set; }
    public float TriggerPercentage { get; set; }
    public bool TrackingToBuy { get; set; }

    public float PriceTrigger => TrackingToBuy
      ? RegularMarketPrice * (1 - TriggerPercentage / 100)
      : RegularMarketPrice * (1 + TriggerPercentage / 100);

    public override bool Equals(object? obj)
    {
      return obj is StockTracking tracking &&
             Symbol == tracking.Symbol &&
             RegularMarketPrice == tracking.RegularMarketPrice &&
             Id == tracking.Id &&
             TriggerPercentage == tracking.TriggerPercentage &&
             TrackingToBuy == tracking.TrackingToBuy &&
             PriceTrigger == tracking.PriceTrigger;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Symbol, RegularMarketPrice, Id, TriggerPercentage, TrackingToBuy, PriceTrigger);
    }
  }
}
