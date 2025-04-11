using Common.DbContexts;
using Microsoft.AspNetCore.Localization;
using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;
using System.Globalization;

const string URL = "http://localhost:5000";
const string SERVICE_NAME = "StockTrackerService";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>();

builder.WebHost.UseUrls(URL);
var app = builder.Build();

var defaultCulture = CultureInfo.InvariantCulture;
var localizationOptions = new RequestLocalizationOptions
{
  DefaultRequestCulture = new RequestCulture(defaultCulture),
  SupportedCultures = [defaultCulture],
  SupportedUICultures = [defaultCulture],
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

if (TaskService.Instance.GetTask(SERVICE_NAME) == null)
{
  var exePath = $"{AppDomain.CurrentDomain.BaseDirectory}{SERVICE_NAME}.exe";
  var taskDefinition = TaskService.Instance.NewTask();
  taskDefinition.RegistrationInfo.Description = $"{SERVICE_NAME} Initializer";
  taskDefinition.Actions.Add(new ExecAction(exePath));
  taskDefinition.Triggers.Add(new LogonTrigger());

  TaskService.Instance.RootFolder.RegisterTaskDefinition(SERVICE_NAME, taskDefinition);
  Process.Start(new ProcessStartInfo { FileName = exePath });
}

app.Run();