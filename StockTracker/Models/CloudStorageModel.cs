using StockTracker.DTOs;
using StockTracker.Models.Shared.Form;
using StockTracker.Models.Shared.Form.Inputs;

namespace StockTracker.Models
{
  public class CloudStorageModel : FormModel
  {
    public const string FORM_ID = "cloud-storage-form";
    public const string BTN_SUBMIT_ID = "btn-submit-cloud-storage";
    public const string EXPORT_MODAL_ID = "export-modal-id";
    public const string BTN_EXPORT_ID = "export-btn-id";
    public const string BTN_IMPORT_ID = "import-btn-id";

    public bool HasString { get; set; }

    public CloudStorageModel(bool hasConnectionString)
      : base(BTN_SUBMIT_ID)
    {
      Id = FORM_ID;
      Inputs = CreateInputs(hasConnectionString);

      if (FormButtonModel != null)
        FormButtonModel.Disabled = hasConnectionString;
    }

    private static InputModel[] CreateInputs(bool hasConnectionString)
    {
      return
      [
        new PasswordInputModel
        {
          Label = "Connection string",
          Name = nameof(CloudStorageDTO.MongoConnectionString),
          IsRequired = true,
          HasContent = hasConnectionString,
        }
      ];
    }
  }
}
