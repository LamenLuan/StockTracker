namespace StockTracker.Models.Shared.Form.Inputs
{
  public class PasswordInputModel : InputModel
  {
    public bool HasContent { get; set; }

    public PasswordInputModel()
    {
      Value = string.Empty;
    }
  }
}
