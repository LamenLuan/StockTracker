namespace StockTrackerConfigurator.DTOs
{
  public class StockSearchDTO
  {
    public string SearchTerm { get; set; }
    public string ApiKey { get; set; }

    public StockSearchDTO()
    {
      SearchTerm = string.Empty;
    }
  }
}
