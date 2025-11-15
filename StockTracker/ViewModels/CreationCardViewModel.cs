using Common.Types;
using StockTrackerConfigurator.DTOs;
using StockTrackerConfigurator.Types;

namespace StockTracker.ViewModels
{
  public class CreationCardViewModel
  {
    public CardType CardType { get; set; }
    public StockTrackDTO? CardInfo { get; set; }

    public static CreationCardViewModel AddCardButton
      => new() { CardType = CardType.ADD };

    public static CreationCardViewModel CardForm
      => new() { CardType = CardType.FORM };

    public CreationCardViewModel() { }

    public CreationCardViewModel(StockTracking stock)
    {
      CardInfo = new StockTrackDTO
      {
        Id = stock.Id,
        Buying = stock.TrackingToBuy,
        Price = stock.RegularMarketPrice,
        StockName = stock.Symbol,
        TriggerPercentage = stock.TriggerPercentage,
        NotificationMuted = stock.NotificationMuted
      };
      CardType = CardType.DETAIL;
    }
  }
}
