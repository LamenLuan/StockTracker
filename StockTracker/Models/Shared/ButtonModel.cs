namespace StockTracker.Models.Shared
{
  public class ButtonModel
  {
    public required string Id { get; set; }
    public string Label { get; set; }
    public bool Disabled { get; set; }

    public ButtonModel()
    {
      Disabled = true;
    }
  }
}
