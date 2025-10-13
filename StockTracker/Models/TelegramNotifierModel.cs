using StockTracker.ViewModels;

namespace StockTracker.Models
{
  public class TelegramNotifierModel
  {
    public const string FORM_ID = "telegram-notifier-form";

    public string? Token { get; set; }
    public long? Id { get; set; }

    public TelegramNotifierModel(TelegramNotifierViewModel viewModel)
    {
      Token = viewModel.Token;
      Id = viewModel.Id;
    }
  }
}
