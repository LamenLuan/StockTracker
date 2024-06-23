namespace StockTrackerConfigurator.Models
{
  public class HomeModel
  {
    public const string API_KEY_FORM_ID = "brapi-key-form";
    public const string API_KEY_INPUT_ID = "brapi-key-input";
    public const string CARDS_ID = "cards";

    public string BrapiKey { get; set; }
    public List<CreationCardModel> Cards { get; set; }

    public HomeModel(List<CreationCardViewModel> cardsViewModel)
    {
      BrapiKey = string.Empty;
      Cards = cardsViewModel.Select(c => new CreationCardModel(c)).ToList();
    }
  }
}
