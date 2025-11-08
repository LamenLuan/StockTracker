using Common.DTOs;
using StockTracker.Models.Shared.Form;
using StockTracker.Models.Shared.Form.Inputs;

namespace StockTracker.Models
{
  public class SettingsModel : FormModel
  {
    public const string FORM_ID = "settings-form";
    public const string BTN_SUBMIT_ID = "btn-submit-settings";

    public SettingsModel(SettingsDTO settingsDTO)
    {
      Id = FORM_ID;
      Inputs = CreateInputs(settingsDTO);
      FormButtonModel = CreateFormButton();
    }

    private static FormButtonModel? CreateFormButton()
    {
      return new FormButtonModel
      {
        Id = BTN_SUBMIT_ID,
        Label = "Save",
        Disabled = false,
      };
    }

    private static InputModel[] CreateInputs(SettingsDTO settingsDTO)
    {
      return
      [
        new TimeInputModel
        {
          Label = "App start time",
          Name = nameof(settingsDTO.AppStartTime),
          Value = settingsDTO.AppStartTime?.ToString() ?? string.Empty,
          IsRequired = true,
        },
        new TimeInputModel
        {
          Label = "App closing time",
          Name = nameof(settingsDTO.AppClosingTime),
          Value = settingsDTO.AppClosingTime?.ToString() ?? string.Empty,
          IsRequired = true,
        },
        new NumberInputModel
        {
          Label = "Price range (%)",
          Name = nameof(settingsDTO.PriceRange),
          Value = settingsDTO.PriceRange?.ToString() ?? string.Empty,
          IsRequired = true,
          Step = 0.1f
        },
        new TimeInputModel
        {
          Label = "Cooldown time",
          Name = nameof(settingsDTO.Cooldown),
          Value = settingsDTO.Cooldown?.ToString() ?? string.Empty,
          IsRequired = true,
        }
      ];
    }
  }
}
