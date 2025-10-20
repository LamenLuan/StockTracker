using Common.DbContexts;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;
using System.Globalization;
using static Common.Constants;


var mutex = new Mutex(true, APP_MUTEX, out var createdNew);
if (!createdNew)
{
  Console.WriteLine("Program already running");
  return;
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>();

builder.WebHost.UseUrls(APP_URL);
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  db.Database.Migrate();
}

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
  FileName = APP_URL,
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
  taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;

  TaskService.Instance.RootFolder.RegisterTaskDefinition(SERVICE_NAME, taskDefinition);
  Process.Start(new ProcessStartInfo { FileName = exePath });
}

app.Run();

mutex.ReleaseMutex();