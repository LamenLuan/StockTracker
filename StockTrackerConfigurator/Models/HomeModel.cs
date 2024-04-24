namespace StockTrackerConfigurator.Models
{
  public class HomeModel
  {
    public const string API_KEY_FORM_ID = "brapi-key-form";
    public const string API_KEY_INPUT_ID = "brapi-key-input";

    public string BrapiKey { get; set; }

    public HomeModel()
    {
      BrapiKey = string.Empty;
    }
  }
}
