namespace StockTrackerConfigurator.DTOs
{
  public class StockTrackDTO
  {
    public long Id { get; set; }
    public string StockName { get; set; }
    public float Price { get; set; }
    public float TriggerPercentage { get; set; }
    public bool Buying { get; set; }

    public StockTrackDTO()
    {
      StockName = string.Empty;
    }
  }
}
