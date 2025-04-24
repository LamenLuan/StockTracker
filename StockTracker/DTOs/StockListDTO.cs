using Common.Types;

namespace StockTrackerConfigurator.DTOs
{
  public class StockListDTO
  {
    public TrackingType Type { get; set; }
    public string[] Stocks { get; set; }

    public StockListDTO()
    {
      Stocks = [];
    }
  }
}
