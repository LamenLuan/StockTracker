
using Common.Types;

namespace Common.DTOs
{
  public class SettingsDTO
  {
    public TimeSpan? AppStartTime { get; set; }
    public TimeSpan? AppClosingTime { get; set; }
    public float? PriceRange { get; set; }
    public TimeSpan? Cooldown { get; set; }

    public static SettingsDTO ParseSettingsToDTO(AppSettings settings)
    {
      return new SettingsDTO
      {
        AppClosingTime = settings.AppClosingTime,
        AppStartTime = settings.AppStartTime,
        Cooldown = settings.Cooldown,
        PriceRange = settings.PriceRange
      };
    }

    public override bool Equals(object? obj)
    {
      return obj is SettingsDTO dTO &&
             EqualityComparer<TimeSpan?>.Default.Equals(AppStartTime, dTO.AppStartTime) &&
             EqualityComparer<TimeSpan?>.Default.Equals(AppClosingTime, dTO.AppClosingTime) &&
             PriceRange == dTO.PriceRange &&
             EqualityComparer<TimeSpan?>.Default.Equals(Cooldown, dTO.Cooldown);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(AppStartTime, AppClosingTime, PriceRange, Cooldown);
    }
  }
}
