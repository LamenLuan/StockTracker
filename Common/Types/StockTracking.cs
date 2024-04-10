namespace Common.Types
{
	public class StockTracking : Stock
	{
		public float TriggerPercentage { get; set; }
		public bool TrackingToBuy { get; set; }
	}
}
