using Common.Extensions;

namespace StockTrackerConfigurator.DTOs
{
  public class ReturnDTO
  {
    public string Content { get; set; }
    public bool Result { get; set; }

    public ReturnDTO()
    {
      Content = string.Empty;
    }

    public static ReturnDTO Success(object content)
    {
      return new ReturnDTO
      {
        Result = true,
        Content = content.Serialize()
      };
    }

    public static ReturnDTO Error(string message)
    {
      return new ReturnDTO
      {
        Result = false,
        Content = message
      };
    }
  }
}
