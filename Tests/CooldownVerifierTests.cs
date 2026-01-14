using Common.Types;
using FluentAssertions;
using StockTrackerService;

namespace Tests
{
  public class CooldownVerifierTests
  {
    private static AppSettings AppSettings => new()
    {
      AppStartTime = new TimeSpan(9, 50, 0),
      AppClosingTime = new TimeSpan(17, 0, 0),
    };

    [Theory]
    [InlineData(9, 49, 59)]
    [InlineData(0)]
    [InlineData(3)]
    public void CooldownNeeded_StartTimeNotReached(int hour, int minute = 0, int second = 0)
    {
      var timeNow = new TimeSpan(hour, minute, second);
      var result = CooldownVerifier.CooldownNeeded(AppSettings, timeNow, out var sleepTime);
      result.Should().Be(true);

      var timeUntilNextStart = AppSettings.AppStartTime - timeNow;
      sleepTime.Should().Be(timeUntilNextStart);
    }

    [Theory]
    [InlineData(17)]
    [InlineData(17, 0, 1)]
    [InlineData(23, 59, 59)]
    public void CooldownNeeded_ClosingTimeReached(int hour, int minute = 0, int second = 0)
    {
      var timeNow = new TimeSpan(hour, minute, second);
      var result = CooldownVerifier.CooldownNeeded(AppSettings, timeNow, out var sleepTime);
      result.Should().Be(true);

      var timeUntilNextStart = TimeSpan.FromDays(1) - timeNow + AppSettings.AppStartTime;
      sleepTime.Should().Be(timeUntilNextStart);
    }

    [Theory]
    [InlineData(9, 50)]
    [InlineData(14, 34, 17)]
    [InlineData(16, 59, 59)]
    public void CooldownNotNeeded(int hour, int minute = 0, int second = 0)
    {
      var timeNow = new TimeSpan(hour, minute, second);
      var result = CooldownVerifier.CooldownNeeded(AppSettings, timeNow, out _);
      result.Should().Be(false);
    }
  }
}
