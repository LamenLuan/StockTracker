namespace StockTrackerConfigurator.Models
{
	public class HomeModel
	{
		public const string API_KEY_FORM_ID = "brapiKeyForm";
		public const string API_KEY_INPUT_ID = "brapiKeyInput";

		public string BrapiKey { get; set; }

		public HomeModel()
		{
			BrapiKey = string.Empty;
		}
	}
}
