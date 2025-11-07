namespace StockTracker.DTOs
{
  public class SettingsDTO
  {
    public TimeSpan? AppStartTime { get; set; }
    public TimeSpan? AppClosingTime { get; set; }
    public float? PriceRange { get; set; }
    public TimeSpan? Cooldown { get; set; }
  }
}
