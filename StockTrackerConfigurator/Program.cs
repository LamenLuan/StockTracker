#if !DEBUG
using System.Diagnostics;
#endif

using Microsoft.AspNetCore.Localization;
using Microsoft.Win32.TaskScheduler;
using System.Globalization;
using static System.Environment;

const string URL = "http://localhost:5000";
const string SERVICE_NAME = "StockTracker";

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

var exePath = $"{GetFolderPath(SpecialFolder.ProgramFiles)}\\{SERVICE_NAME}\\{SERVICE_NAME}.exe";

if (TaskService.Instance.GetTask(SERVICE_NAME) == null)
{
  var taskDefinition = TaskService.Instance.NewTask();
  taskDefinition.RegistrationInfo.Description = $"{SERVICE_NAME} Initializer";
  taskDefinition.Actions.Add(new ExecAction(exePath));
  taskDefinition.Triggers.Add(new LogonTrigger { });

  TaskService.Instance.RootFolder.RegisterTaskDefinition(SERVICE_NAME, taskDefinition);
}

app.Run();