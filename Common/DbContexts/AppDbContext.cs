using Common.Types;
using Microsoft.EntityFrameworkCore;

namespace Common.DbContexts
{
  public class AppDbContext : DbContext
  {
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected DbSet<AppSettings> AppSettings { get; set; } = null!;
    protected DbSet<StockTracking> StockTrackings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<AppSettings>().HasKey(x => x.Id);
      modelBuilder.Entity<StockTracking>().HasKey(x => x.Id);
      base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.db");
      optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    public async Task<AppSettings> GetSettings()
    {
      var settings = AppSettings.FirstOrDefault();

      if (settings == null)
      {
        settings = new AppSettings();
        AppSettings.Add(settings);
        await SaveChangesAsync();
      }

      return settings;
    }

    public async Task<StockTracking?> GetStockTrackingAsync(long id)
    {
      var stockTrackings = await StockTrackings.FindAsync(id);
      return stockTrackings;
    }

    public async Task<List<StockTracking>> GetStockTrackingsAsync()
    {
      var stockTrackings = await StockTrackings.ToListAsync();
      return stockTrackings;
    }

    public virtual async Task AddStockTracking(StockTracking stockTracking)
    {
      StockTrackings.Add(stockTracking);
      await SaveChangesAsync();
    }

    public virtual async Task RemoveStockTracking(StockTracking stockTracking)
    {
      StockTrackings.Remove(stockTracking);
      await SaveChangesAsync();
    }

    public virtual async Task SaveApiKey(string apiKey)
    {
      var settings = await GetSettings();
      settings.ApiKey = apiKey;
      await SaveChangesAsync();
    }

    public virtual async Task SaveTelegramInfo(string botToken, long id)
    {
      var settings = await GetSettings();
      settings.TelegramBotToken = botToken;
      settings.TelegramId = id;
      await SaveChangesAsync();
    }
  }
}
