using Common.Types;
using FluentAssertions;
using StockTrackerService;

namespace Tests
{
  public class PriceTriggeringTests
  {
    private static StockTracking TestStockTracking => new()
    {
      Id = 1,
      Symbol = "ITUB4F",
      RegularMarketPrice = 10f
    };

    #region PriceTriggered

    [Theory]
    [InlineData(0f, false)]
    [InlineData(-10f, false)]
    [InlineData(9.99f, true)]
    [InlineData(10f, true)]
    [InlineData(10.01f, false)]
    public void PriceTriggered_ToBuy_WhenParamsValid(float priceNow, bool expected)
    {
      var stockTracking = TestStockTracking;
      stockTracking.TrackingToBuy = true;
      var result = Program.PriceTriggered(stockTracking, priceNow, 0f);
      result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0f, false)]
    [InlineData(-10f, false)]
    public void PriceTriggered_ToBuy_WhenParamsInvalid(float priceNow, bool expected)
    {
      var stockTracking = TestStockTracking;
      stockTracking.TrackingToBuy = true;
      var result = Program.PriceTriggered(stockTracking, priceNow, 0f);
      result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0f, false)]
    [InlineData(-10f, false)]
    [InlineData(9.99f, false)]
    [InlineData(10f, true)]
    [InlineData(10.01f, true)]
    public void PriceTriggered_ToSell_WhenParamsValid(float priceNow, bool expected)
    {
      var stockTracking = TestStockTracking;
      stockTracking.TrackingToBuy = false;
      var result = Program.PriceTriggered(stockTracking, priceNow, 0f);
      result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0f, false)]
    [InlineData(-10f, false)]
    public void PriceTriggered_ToSell_WhenParamsInvalid(float priceNow, bool expected)
    {
      var stockTracking = TestStockTracking;
      stockTracking.TrackingToBuy = false;
      var result = Program.PriceTriggered(stockTracking, priceNow, 0f);
      result.Should().Be(expected);
    }

    #endregion

    #region PriceAlmostTriggering

    [Theory]
    [InlineData(9f, 1f, true)]
    [InlineData(10.1f, 1f, true)]
    [InlineData(10.11f, 1f, false)]
    [InlineData(11f, 1f, false)]
    public void PriceAlmostTriggering_ToBuy_WhenParamsValid(float priceNow, float priceRange, bool expected)
    {
      var stockTracking = TestStockTracking;
      stockTracking.TrackingToBuy = true;
      var result = Program.PriceTriggered(stockTracking, priceNow, priceRange);
      result.Should().Be(expected);
    }

    [Theory]
    [InlineData(11f, 1f, true)]
    [InlineData(9.9f, 1f, true)]
    [InlineData(9.89f, 1f, false)]
    [InlineData(9f, 1f, false)]
    public void PriceAlmostTriggering_ToSell_WhenParamsValid(float priceNow, float priceRange, bool expected)
    {
      var stockTracking = TestStockTracking;
      stockTracking.TrackingToBuy = false;
      var result = Program.PriceTriggered(stockTracking, priceNow, priceRange);
      result.Should().Be(expected);
    }

    #endregion

  }
}