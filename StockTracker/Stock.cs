using System.Globalization;

namespace StockTracker
{
	public class Stock
	{
		public string Code { get; set; } = string.Empty;
		public float Price { get; set; }

		public override string ToString()
		{
			if (string.IsNullOrEmpty(Code) || Price <= 0)
				throw new Exception();

			return $"{Code},{Price.ToString("#.##", CultureInfo.InvariantCulture)}";
		}
	}
}
