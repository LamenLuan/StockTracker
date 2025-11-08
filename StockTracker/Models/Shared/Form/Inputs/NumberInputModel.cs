namespace StockTracker.Models.Shared.Form.Inputs
{
  public class NumberInputModel : InputModel
  {
    public int? Min { get; set; }
    public int? Max { get; set; }
    public float Step { get; set; }

    public NumberInputModel()
    {
      Step = 1f;
    }
  }
}
