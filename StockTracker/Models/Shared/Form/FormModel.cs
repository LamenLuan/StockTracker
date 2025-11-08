using StockTracker.Models.Shared.Form.Inputs;

namespace StockTracker.Models.Shared.Form
{
  public abstract class FormModel
  {
    public string Id { get; set; }
    public InputModel[] Inputs { get; set; }
    public FormButtonModel? FormButtonModel { get; set; }

    protected FormModel()
    {
      Id = string.Empty;
      Inputs = [];
    }

    protected FormModel(string buttonId) : this()
    {
      FormButtonModel = new FormButtonModel
      {
        Id = buttonId,
        Label = "Save",
        Disabled = false,
      };
    }
  }
}
