using Common.Types;
using System.Diagnostics;

namespace StockTrackerService
{
  public class CooldownVerifier
  {
    public static void WaitUntilStartTime(AppSettings appSettings)
    {
      if (Debugger.IsAttached) return;

      var timeNow = DateTime.Now.TimeOfDay;
      if (CooldownNeeded(appSettings, timeNow, out var sleepTime))
        Thread.Sleep(sleepTime);
    }

    public static bool CooldownNeeded(
      AppSettings appSettings,
      TimeSpan timeNow,
      out TimeSpan sleepTime)
    {
      if (timeNow < appSettings.AppStartTime)
      {
        sleepTime = appSettings.AppStartTime - timeNow;
        return true;
      }

      if (timeNow >= appSettings.AppClosingTime)
      {
        sleepTime = TimeSpan.FromDays(1) - timeNow + appSettings.AppStartTime;
        return true;
      }

      sleepTime = TimeSpan.Zero;
      return false;
    }
  }
}
