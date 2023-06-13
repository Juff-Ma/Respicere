namespace Respicere.Server.Helpers;

using Configuration = Shared.Configuration;

public static class ConfigurationExtensions
{
    public static int GetWidth(this Configuration configuration)
    {
        return int.Parse(configuration.CameraResolution?.Split('x')[0] ?? "");
    }

    public static int GetHeight(this Configuration configuration)
    {
        return int.Parse(configuration.CameraResolution?.Split('x')[1] ?? "");
    }

    public static int GetFps(this Configuration configuration)
    {
        return int.Parse(configuration.CameraFps ?? "");
    }
}
