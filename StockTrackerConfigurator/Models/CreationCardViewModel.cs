using Common.Types;
using StockTrackerConfigurator.DTOs;

namespace StockTrackerConfigurator.Models
{
  public class CreationCardViewModel
  {
    public bool AddCardButtonMode { get; set; }
    public StockTrackDTO? CardInfo { get; set; }

    public static CreationCardViewModel AddCardButton
      => new() { AddCardButtonMode = true };

    public CreationCardViewModel() { }

    public CreationCardViewModel(StockTracking stock)
    {
      CardInfo = new StockTrackDTO
      {
        Buying = stock.TrackingToBuy,
        Price = stock.RegularMarketPrice,
        StockName = stock.Symbol,
        TriggerPercentage = stock.TriggerPercentage
      };
    }
  }
}
