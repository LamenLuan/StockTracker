using Common.Extensions;
using Common.Types;
using static System.Environment;

namespace Common
{
	public static class FileManager
	{
		private const string DIRECTORY_NAME = "StockTracker";
		private const string STOCKS_FILE_NAME = "stocks.dt";
		private const string API_KEY_FILE_NAME = "brapiKey.dt";

		private static string DirectoryPath
			=> $"{GetFolderPath(SpecialFolder.MyDocuments)}/{DIRECTORY_NAME}";
		private static string StocksPath
			=> $"{DirectoryPath}/{STOCKS_FILE_NAME}";
		private static string ApiKeyPath
			=> $"{DirectoryPath}/{API_KEY_FILE_NAME}";

		public static string ReadBrapiKey()
		{
			if (!File.Exists(ApiKeyPath)) return string.Empty;
			return File.ReadAllText(ApiKeyPath);
		}

		public static List<StockTracking> ReadStockTrackings()
		{
			if (!File.Exists(StocksPath)) return new();
			var fileText = File.ReadAllText(StocksPath);
			return fileText.Deserialize<List<StockTracking>>() ?? new();
		}

		public static void WriteBrapiKey(string key)
		{
			File.WriteAllText(ApiKeyPath, key);
		}
	}
}