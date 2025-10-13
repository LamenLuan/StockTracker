namespace StockTracker.ViewModels
{
  public class TelegramNotifierViewModel
  {
    public string? Token { get; set; }
    public long? Id { get; set; }

    public TelegramNotifierViewModel()
    {
      Token = string.Empty;
    }
  }
}
