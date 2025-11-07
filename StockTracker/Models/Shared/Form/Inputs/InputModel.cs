namespace StockTracker.Models.Shared.Form.Inputs
{
  public abstract class InputModel
  {
    public required string Value { get; set; }
    public required string Label { get; set; }
    public required string Name { get; set; }
    public bool IsRequired { get; set; }
  }
}
