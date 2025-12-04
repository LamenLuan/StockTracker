using Common.Types;
using FluentAssertions;
using StockTrackerService;

namespace Tests
{
  public class StockTrackerServiceTests
  {
    private static StockTracking TestStockTracking => new()
    {
      Id = 1,
      Symbol = "ITUB4F",
      RegularMarketPrice = 10f
    };

    [Theory]
    [InlineData(9.99f, 0f, true)]
    [InlineData(10f, 0f, true)]
    [InlineData(10.01f, 0f, false)]
    public void PriceTriggeredToBuy_WhenParamsValid(float priceNow, float priceRange, bool expected)
    {
      var stockTracking = TestStockTracking;
      stockTracking.TrackingToBuy = true;
      var result = Program.PriceTriggered(stockTracking, priceNow, priceRange);
      result.Should().Be(expected);
    }
  }
}