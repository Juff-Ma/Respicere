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

    public static string GetPhotoCronCycle(this Configuration configuration) =>
        configuration.PhotoCronCycle ?? "0/10 * * ? * * *";
}
