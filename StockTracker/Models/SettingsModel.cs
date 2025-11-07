using StockTracker.DTOs;
using StockTracker.Models.Shared.Form;
using StockTracker.Models.Shared.Form.Inputs;

namespace StockTracker.Models
{
  public class SettingsModel : FormModel
  {
    public const string FORM_ID = "cloud-storage-form";
    public const string BTN_SUBMIT_ID = "btn-submit-cloud-storage";
    public const string EXPORT_MODAL_ID = "export-modal-id";
    public const string BTN_EXPORT_ID = "export-btn-id";
    public const string BTN_IMPORT_ID = "import-btn-id";

    public SettingsDTO Settings { get; set; }

    public SettingsModel(SettingsDTO settingsDTO)
    {
      Id = FORM_ID;
      Settings = settingsDTO;
      Inputs = CreateInputs(settingsDTO);
    }

    private static InputModel[] CreateInputs(SettingsDTO settingsDTO)
    {
      return
      [
        new TimeInputModel
        {
          Label = "App Start Time",
          Name = nameof(settingsDTO.AppStartTime),
          Value = settingsDTO.AppStartTime?.ToString() ?? string.Empty,
          IsRequired = true,
        },
        new TimeInputModel
        {
          Label = "App Closing Time",
          Name = nameof(settingsDTO.AppClosingTime),
          Value = settingsDTO.AppClosingTime?.ToString() ?? string.Empty,
          IsRequired = true,
        },
        new NumberInputModel
        {
          Label = "Price Range",
          Name = nameof(settingsDTO.PriceRange),
          Value = settingsDTO.PriceRange?.ToString() ?? string.Empty,
          IsRequired = true,
        },
        new NumberInputModel
        {
          Label = "Cooldown (in minutes)",
          Name = nameof(settingsDTO.Cooldown),
          Value = settingsDTO.Cooldown?.ToString() ?? string.Empty,
          IsRequired = true,
        }
      ];
    }
  }
}
