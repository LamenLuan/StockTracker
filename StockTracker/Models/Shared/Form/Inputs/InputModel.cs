namespace StockTracker.Models.Shared.Form.Inputs
{
  public abstract class InputModel
  {
    public string Value { get; set; }
    public string Label { get; set; }
    public string Name { get; set; }
    public bool IsRequired { get; set; }

    protected InputModel()
    {
      Value = string.Empty;
      Label = string.Empty;
      Name = string.Empty;
    }
  }
}
