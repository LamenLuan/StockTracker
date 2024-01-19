using StockTracker.Types;
using StockTracker.Utils.Extensions;
using System.Text.Json;

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

    public static List<StockTracking> ReadStockTrackings()
    {
      if (!File.Exists(Path)) return new();
      var fileText = File.ReadAllText(Path);
      return fileText.Deserialize<List<StockTracking>>() ?? new();
    }
  }
}