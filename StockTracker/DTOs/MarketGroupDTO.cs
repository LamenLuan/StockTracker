using Common.Types;

namespace StockTracker.DTOs
{
  public class MarketGroupDTO
  {
    public TrackingType Type { get; set; }
    public string[] Items { get; set; }

    public MarketGroupDTO()
    {
      Items = [];
    }
  }
}
