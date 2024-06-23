namespace StockTrackerConfigurator.DTOs
{
  public class StockListDTO
  {
    public string[] Stocks { get; set; }

    public StockListDTO()
    {
      Stocks = new string[0];
    }
  }
}
