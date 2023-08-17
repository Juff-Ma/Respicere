using FlashCap;
using Respicere.Server.Helpers;
using Respicere.Server.Data;
using Configuration = Respicere.Shared.Configuration;
using Microsoft.EntityFrameworkCore;
using Respicere.Server.Interfaces;
using Respicere.Server;
using Quartz;
using Respicere.Server.Jobs;
using Respicere.Server.Models;
using FFMediaToolkit;
using Respicere.Server.Processors;
using Respicere.Server.Services;

var builder = WebApplication.CreateBuilder(args);

var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
}).CreateLogger("Startup");

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
    logger.LogCritical(e, "Couldn't start app due to an Error while creating video device, please check your configuration");
    cameraAcessible = false;
    if (!builder.Environment.IsDevelopment())
    {
        Environment.Exit(-1);
    }
}

if (options.GetUseVideo() && cameraAcessible)
{
    try
    {
        if (options.GetUseOwnFFmpegBinaries())
            FFmpegLoader.FFmpegPath = options.GetFFmpegPath();
        FFmpegLoader.LoadFFmpeg();
        FFmpegLoader.SetupLogging();
        builder.Services.AddSingleton<FFmpegLogWrapper>();
    }
    catch (Exception e)
    {
        logger.LogError(e, "Couldn't initialize FFmpeg!");
    }
}

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.ConfigureWritable<Configuration>(builder.Configuration.GetSection("Configuration"));

if (cameraAcessible)
{
    var cam = new Cam(deviceDescriptor!, characteristics!);
    builder.Services.AddSingleton<ICam>(cam);
    builder.Services.AddSingleton<IDataProducer<IDataProcessor>>(cam);

    builder.Services.AddSingleton<IDataProducer<MjpegProcessor>, PreprocessorWrapper<MjpegProcessor>>();
    builder.Services.AddSingleton<IDataProducer<VideoProcessor>, PreprocessorWrapper<VideoProcessor>>();

    builder.Services.AddSingleton<IVideoWriter, VideoWriterService>();

    builder.Services.AddTransient<TakePhotoJob>();
    builder.Services.AddTransient<DeletePhotosJob>();
}

var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddDbContext<DataDbContext>(DbOptions => {
    switch (options.GetDbType())
    {
        case DbType.SQLite:
            DbOptions.UseSqlite(connectionString); break;
        case DbType.SQLServer:
            DbOptions.UseSqlServer(connectionString); break;
        case DbType.PostgreSQL:
            DbOptions.UseNpgsql(connectionString); break;
        case DbType.MySQL:
            DbOptions.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)); break;
        default:
            DbOptions.UseSqlite(connectionString); break;
    }
});

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

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

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