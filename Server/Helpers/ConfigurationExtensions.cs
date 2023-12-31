﻿// Ignore Spelling: Cron

namespace Respicere.Server.Helpers;

using Configuration = Shared.Configuration;
using FlashCap.Utilities;
using Models;

public static class ConfigurationExtensions
{
    public static int GetWidth(this Configuration configuration) =>
        int.Parse(configuration.CameraResolution?.Split('x')[0] ?? "1280");

    public static int GetHeight(this Configuration configuration) =>
        int.Parse(configuration.CameraResolution?.Split('x')[1] ?? "720");

    public static Fraction GetFps(this Configuration configuration) =>
        new(int.Parse(configuration.CameraFpsNumerator ?? "30"),
            int.Parse(configuration.CameraFpsDenominator ?? "1"));

    public static bool GetPhotoEnabled(this Configuration configuration) =>
        bool.Parse(configuration.PhotoEnabled ?? "false");

    public static string GetPhotoPath(this Configuration configuration) =>
        configuration.PhotoPath ?? "./photos";

    public static string GetPhotoTakeCronCycle(this Configuration configuration) =>
        configuration.PhotoTakeCronCycle ?? "0/10 * * ? * * *";

    public static string GetPhotoDeleteCronCycle(this Configuration configuration) =>
        configuration.PhotoDeleteCronCycle ?? "0 0 0 ? * 1/3 *";

    public static TimeSpan GetPhotoDeleteOlderThan(this Configuration configuration) =>
        TimeSpan.TryParse(configuration.PhotoDeleteOlderThan ?? "2.00:00", out TimeSpan span) ? span : TimeSpan.FromDays(2);

    public static DbType GetDbType(this Configuration configuration) => configuration.DatabaseType?.ToLower() switch
    {
        "sqlite" => DbType.SQLite,
        "sqlserver" => DbType.SQLServer,
        "postgresql" => DbType.PostgreSQL,
        "mysql" => DbType.MySQL,
        "mariadb" => DbType.MySQL,
        _ => DbType.SQLite
    };
}
