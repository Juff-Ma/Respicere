// Ignore Spelling: Cron Fmpeg

namespace Respicere.Server.Helpers;

using FlashCap.Utilities;
using Models;
using Shared;
using System.Globalization;

public static class ConfigurationExtensions
{
    public static int GetWidth(this Configuration configuration) =>
        int.TryParse(configuration.CameraResolution?.Split('x')[0] ?? "1280", out int width) ? width : 1280;

    public static int GetHeight(this Configuration configuration) =>
        int.TryParse(configuration.CameraResolution?.Split('x')[1] ?? "720", out int height) ? height : 720;

    public static Fraction GetFps(this Configuration configuration) =>
        new(int.TryParse(configuration.CameraFpsNumerator ?? "30", out int numerator) ? numerator : 30,
            int.TryParse(configuration.CameraFpsDenominator ?? "1", out int denominator) ? denominator : 1);

    public static bool GetPhotoEnabled(this Configuration configuration) =>
        bool.TryParse(configuration.PhotoEnabled ?? "false", out bool result) && result;

    public static string GetPhotoPath(this Configuration configuration) =>
        configuration.PhotoPath ?? "./photos";

    public static string GetPhotoTakeCronCycle(this Configuration configuration) =>
        configuration.PhotoTakeCronCycle ?? "0/10 * * ? * * *";

    public static string GetPhotoDeleteCronCycle(this Configuration configuration) =>
        configuration.PhotoDeleteCronCycle ?? "0 0 0 ? * 1/3 *";

    public static TimeSpan GetPhotoDeleteOlderThan(this Configuration configuration) =>
        TimeSpan.TryParse(configuration.PhotoDeleteOlderThan ?? "2.00:00", CultureInfo.InvariantCulture, out TimeSpan span) ? span : TimeSpan.FromDays(2);

    public static DbType GetDbType(this Configuration configuration) => configuration.DatabaseType?.ToLower() switch
    {
        "sqlite" => DbType.SQLite,
        "sqlserver" => DbType.SQLServer,
        "postgresql" => DbType.PostgreSQL,
        "mysql" => DbType.MySQL,
        "mariadb" => DbType.MySQL,
        _ => DbType.SQLite
    };

    public static bool GetUseVideo(this Configuration configuration) =>
        bool.TryParse(configuration.UseVideo ?? "false", out bool result) && result;

    public static bool GetUseOwnFFmpegBinaries(this Configuration configuration) =>
        bool.TryParse(configuration.UseOwnFFmpegBinaries ?? "false", out bool result) && result;

    public static string GetFFmpegPath(this Configuration configuration) =>
        Directory.Exists(configuration.FFmpegPath) ? configuration.FFmpegPath : "./ffmpeg";
}
