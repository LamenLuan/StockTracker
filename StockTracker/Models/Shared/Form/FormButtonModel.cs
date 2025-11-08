namespace StockTracker.Models.Shared.Form
{
  public class FormButtonModel
  {
    public required string Id { get; set; }
    public string Label { get; set; }
    public bool Disabled { get; set; }

    public FormButtonModel()
    {
      Label = string.Empty;
    }
  }
}
