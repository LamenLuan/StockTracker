namespace StockTrackerConfigurator.DTOs
{
  public class StockSearchDTO
  {
    public string SearchTerm { get; set; }

    public StockSearchDTO()
    {
      SearchTerm = string.Empty;
    }
  }
}
