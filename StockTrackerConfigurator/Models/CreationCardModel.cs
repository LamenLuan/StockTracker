namespace StockTrackerConfigurator.Models
{
  public class CreationCardModel
  {
    public const string FORM_ID = "stock-form";
    public const string STOCK_INPUT_ID = "stock";
    public const string PRICE_INPUT_ID = "price";
    public const string PERCENTAGE_INPUT_ID = "percentage";
    public const string PERCENTAGE_RESULT_INPUT_ID = "percentage-result";
    public const string ADD_CARD_ID = "add-card";

    public bool AddCardButtonMode { get; set; }

    public CreationCardModel(bool addCard = false)
    {
      AddCardButtonMode = addCard;
    }

    public CreationCardModel(CreationCardViewModel viewModel)
    {
      AddCardButtonMode = viewModel.AddCardButtonMode;
    }
  }
}
