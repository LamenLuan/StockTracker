using Common.Types;

namespace StockTracker.DTOs
{
  public class StockSearchReturnDTO
  {
    public MarketGroupDTO Stocks { get; set; }
    public MarketGroupDTO Coins { get; set; }

    public StockSearchReturnDTO()
    {
      Stocks = new MarketGroupDTO { Type = TrackingType.STOCK };
      Coins = new MarketGroupDTO { Type = TrackingType.CRYPTO };
    }
  }
}
