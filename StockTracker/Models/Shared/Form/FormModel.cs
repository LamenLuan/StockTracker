using StockTracker.Models.Shared.Form.Inputs;

namespace StockTracker.Models.Shared.Form
{
  public abstract class FormModel
  {
    public required string Id { get; set; }
    public required InputModel[] Inputs { get; set; }
  }
}
