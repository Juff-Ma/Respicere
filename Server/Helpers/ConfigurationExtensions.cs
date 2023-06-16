// Ignore Spelling: Cron

namespace Respicere.Server.Helpers;

using Configuration = Shared.Configuration;

public static class ConfigurationExtensions
{
    public static int GetWidth(this Configuration configuration) =>
        int.Parse(configuration.CameraResolution?.Split('x')[0] ?? "1280");

    public static int GetHeight(this Configuration configuration) =>
        int.Parse(configuration.CameraResolution?.Split('x')[1] ?? "720");

    public static int GetFps(this Configuration configuration) =>
        int.Parse(configuration.CameraFps ?? "30");

    public static bool GetPhotoEnabled(this Configuration configuration) =>
        bool.Parse(configuration.PhotoEnabled ?? "false");

    public static string GetPhotoPath(this Configuration configuration) =>
        configuration.PhotoPath ?? "./photos";

    public static string GetPhotoTakeCronCycle(this Configuration configuration) =>
        configuration.PhotoTakeCronCycle ?? "0/10 * * ? * * *";

    public static string GetPhotoDeleteCronCycle(this Configuration configuration) =>
        configuration.PhotoDeleteCronCycle ?? "0 0 0 ? * 1/3 *";

    public static TimeSpan GetPhotoDeleteOlderThan(this Configuration configuration) =>
        TimeSpan.Parse(configuration.PhotoDeleteOlderThan ?? "2.00:00");
}
