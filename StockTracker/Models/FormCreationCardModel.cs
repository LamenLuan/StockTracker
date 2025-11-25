using StockTrackerConfigurator.Types;

namespace StockTracker.Models
{
  public class FormCreationCardModel : CreationCardModel
  {
    public string[] StockNames { get; set; }

    public FormCreationCardModel(string[] stockNames)
    {
      Type = CardType.FORM;
      StockNames = stockNames;
    }
  }
}
