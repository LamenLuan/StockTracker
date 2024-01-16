namespace StockTracker
{
	public static class FileManager
	{
		private const string DIRECTORY_NAME = "StockTracker";
		private const string FILE_NAME = "stocks.dt";

		private static string DirectoryPath
			=> $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/{DIRECTORY_NAME}";
		private static string Path
			=> $"{DirectoryPath}/{FILE_NAME}";

		public static string[]? ReadFileData()
		{
			if (!File.Exists(Path))
				return null;

			return File.ReadAllLines(Path);
		}

		public static void WriteStock(Stock stock, bool clearBefore = false)
		{
			if (!File.Exists(Path))
				CreateFile();

			var sw = new StreamWriter(Path, !clearBefore);
			sw.WriteLine(stock);
			sw.Close();
		}

		private static void CreateFile()
		{
			if (!Directory.Exists(DirectoryPath))
				Directory.CreateDirectory(DirectoryPath);
		}
	}
}