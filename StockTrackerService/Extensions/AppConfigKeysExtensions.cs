using StockTracker.Types;
using System.Configuration;
using System.Globalization;

namespace StockTracker.Extensions
{
	public static class AppConfigKeysExtensions
	{
		private static string GetName(this AppConfigKeys key) => key switch
		{
			AppConfigKeys.BRAPI_KEY => "Brapikey",
			AppConfigKeys.COOLDOWN => "Cooldown",
			AppConfigKeys.START_TIME => "StartTime",
			AppConfigKeys.END_TIME => "EndTime",
			AppConfigKeys.NEAR_PRICE_RANGE => "NearPriceRangePctg",
			_ => throw new()
		};

		public static string? GetValue(this AppConfigKeys key)
			=> ConfigurationManager.AppSettings[key.GetName()];

		public static TimeSpan GetAsTimeSpan(this AppConfigKeys appKey)
		{
			try
			{
				if (!TimeSpan.TryParse(appKey.GetValue(), out var setting))
					throw new ArgumentOutOfRangeException(appKey.GetName());

				return setting;
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Notifier.Notify($"Program failed to load the setting [{ex.ParamName}]. Program closed");
				throw;
			}
		}

		public static float GetAsFloat(this AppConfigKeys appKey)
		{
			try
			{
				if (!float.TryParse(appKey.GetValue(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var setting))
					throw new ArgumentOutOfRangeException(appKey.GetName());

				return setting;
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Notifier.Notify($"Program failed to load the setting [{ex.ParamName}]. Program closed");
				throw;
			}
		}
	}
}
