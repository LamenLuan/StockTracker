namespace Common.Types
{
  public class AppSettings
  {
    public long Id { get; set; }
    public string? ApiKey { get; set; }
    public string? TelegramBotToken { get; set; }
    public long? TelegramId { get; set; }
  }
}
