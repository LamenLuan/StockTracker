using Common.Types;
using Microsoft.EntityFrameworkCore;

namespace Common.DbContexts
{
  public class AppDbContext : DbContext
  {
    private DbSet<AppSettings> AppSettings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<AppSettings>().HasKey(x => x.Id);
      base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite("Data Source=data.db");
    }

    public async Task<AppSettings> GetSettings()
    {
      var settings = AppSettings.FirstOrDefault();

      if (settings == null)
      {
        settings = new AppSettings();
        AppSettings.Add(settings);
      }

      await SaveChangesAsync();

      return settings;
    }
  }
}
