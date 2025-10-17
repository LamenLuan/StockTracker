namespace StockTracker.DTOs
{
  public class TelegramNotifierDTO
  {
    public string Token { get; set; }

    public TelegramNotifierDTO()
    {
      Token = string.Empty;
    }
  }
}
