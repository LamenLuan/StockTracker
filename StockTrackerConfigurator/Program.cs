#if !DEBUG
using System.Diagnostics;
#endif

using Microsoft.AspNetCore.Localization;
using System.Globalization;

const string URL = "http://localhost:5000";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.WebHost.UseUrls(URL);
var app = builder.Build();

var defaultCulture = CultureInfo.InvariantCulture;
var localizationOptions = new RequestLocalizationOptions
{
  DefaultRequestCulture = new RequestCulture(defaultCulture),
  SupportedCultures = new List<CultureInfo> { defaultCulture },
  SupportedUICultures = new List<CultureInfo> { defaultCulture },
  ApplyCurrentCultureToResponseHeaders = true
};
app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

#if !DEBUG
Process.Start(new ProcessStartInfo
{
  FileName = URL,
  UseShellExecute = true
});
#endif

app.Run();
