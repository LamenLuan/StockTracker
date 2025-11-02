namespace StockTracker.DTOs
{
  public class CloudStorageDTO
  {
    public string? MongoConnectionString { get; set; }
    public bool? OverwriteLocalData { get; set; }
  }
}
