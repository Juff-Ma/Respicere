using FlashCap;
using Respicere.Server.Helpers;
using Respicere.Server.Data;
using Configuration = Respicere.Shared.Configuration;
using Microsoft.EntityFrameworkCore;
using Respicere.Server.Interfaces;
using Respicere.Server;
using Quartz;
using Respicere.Server.Jobs;

var builder = WebApplication.CreateBuilder(args);

var options = new Configuration();
builder.Configuration.Bind("Configuration", options);

CaptureDeviceDescriptor? deviceDescriptor = null;
VideoCharacteristics? characteristics = null;

bool cameraAcessible = true;
try
{
    var captureDevices = new CaptureDevices();
    deviceDescriptor = captureDevices.EnumerateDescriptors().First(x => x.Characteristics.Length > 0);
    characteristics = deviceDescriptor.Characteristics
        .First(x => x.FramesPerSecond == options.GetFps() &&
                x.Height == options.GetHeight() &&
                x.Width == options.GetWidth() &&
                x.PixelFormat != PixelFormats.Unknown);
}
catch (Exception e)
{
    Console.WriteLine("Couldn't start app due to an Error while creating video device, please check your configuration ");
    Console.WriteLine(e.Message);
    Console.WriteLine(e.StackTrace);
    cameraAcessible = false;
    if (!builder.Environment.IsDevelopment())
    {
        return;
    }
}

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.ConfigureWritable<Configuration>(builder.Configuration.GetSection("Configuration"));

if (cameraAcessible)
{
    builder.Services.AddSingleton<ICam>(new Cam(deviceDescriptor!, characteristics!));
    builder.Services.AddTransient<TakePhotoJob>();
    builder.Services.AddTransient<DeletePhotosJob>();
}

var appExecutable = System.Reflection.Assembly.GetExecutingAssembly().Location;
var path = Path.GetDirectoryName(appExecutable)!;
var dbPath = Path.Join(path, "data.db");

builder.Services.AddDbContext<DataDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddQuartz(quartz =>
{
    quartz.UseMicrosoftDependencyInjectionJobFactory();

    var takePhotoJobKey = new JobKey("TakePhotoJob");
    var deletePhotosJobKey = new JobKey("DeletePhotosJob");

    if (options.GetPhotoEnabled())
    {
        quartz.AddJob<TakePhotoJob>(options => options.WithIdentity(takePhotoJobKey));
        quartz.AddTrigger(triggerOptions => triggerOptions
            .ForJob(takePhotoJobKey)
            .WithIdentity("TakePhotoJobTrigger")
            .WithCronSchedule(options.GetPhotoTakeCronCycle()));

        quartz.AddJob<DeletePhotosJob>(options => options.WithIdentity(deletePhotosJobKey));
        quartz.AddTrigger(triggerOptions => triggerOptions
            .ForJob(deletePhotosJobKey)
            .WithIdentity("DeletePhotosJobTrigger")
            .WithCronSchedule(options.GetPhotoDeleteCronCycle()));
    }
});

builder.Services.AddQuartzServer(options => options.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();