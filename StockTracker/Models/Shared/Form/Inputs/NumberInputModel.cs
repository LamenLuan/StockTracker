namespace StockTracker.Models.Shared.Form.Inputs
{
  public class NumberInputModel : InputModel
  {
    public float? Min { get; set; }
    public float? Max { get; set; }
    public float Step { get; set; }

    public NumberInputModel()
    {
      Step = 1f;
    }
  }
}
