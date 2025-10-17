namespace StockTracker.Models
{
  public class TelegramNotifierModel
  {
    public const string FORM_ID = "telegram-notifier-form";
    public const string BTN_SUBMIT_ID = "btn-submit-notifier";
    public const string HELP_TEXT_ID = "text-help-notifier";

    public bool HasToken { get; set; }
  }
}
