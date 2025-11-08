using StockTracker.DTOs;
using StockTracker.Models.Shared.Form;
using StockTracker.Models.Shared.Form.Inputs;

namespace StockTracker.Models
{
  public class TelegramNotifierModel : FormModel
  {
    public const string FORM_ID = "telegram-notifier-form";
    public const string BTN_SUBMIT_ID = "btn-submit-notifier";
    public const string HELP_TEXT_ID = "text-help-notifier";

    public bool HasToken { get; set; }

    public TelegramNotifierModel(bool hasConfig)
      : base(BTN_SUBMIT_ID)
    {
      Id = FORM_ID;
      Inputs = CreateInputs(hasConfig);

      if (FormButtonModel != null)
        FormButtonModel.Disabled = hasConfig;
    }

    private static InputModel[] CreateInputs(bool hasToken)
    {
      return
      [
        new PasswordInputModel
        {
          Label = "Bot token",
          Name = nameof(TelegramNotifierDTO.Token),
          IsRequired = true,
          HasContent = hasToken,
        }
      ];
    }
  }
}
