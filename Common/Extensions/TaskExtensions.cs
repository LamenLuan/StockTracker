namespace Common.Extensions
{
  public static class TaskExtensions
  {
    public static async Task<T?> GetResultAsync<T>(this Task<T> task)
    {
      try
      {
        return await task ?? default;
      }
      catch (Exception)
      {
        return default;
      }
    }

    public static T? GetResult<T>(this Task<T> task)
      => task.GetResultAsync().Result;
  }
}
