namespace StockTracker.DTOs
{
  public class CryptoListDTO
  {
    public string[] Coins { get; set; }

    public CryptoListDTO()
    {
      Coins = [];
    }
  }
}
