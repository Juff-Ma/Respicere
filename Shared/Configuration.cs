// Ignore Spelling: Cron Fmpeg

namespace Respicere.Shared;

public class Configuration
{
    public string? CameraResolution { get; set; }
    public string? CameraFpsNumerator { get; set;}
    public string? CameraFpsDenominator { get; set; }
    public string? PhotoPath { get; set;}
    public string? PhotoTakeCronCycle { get; set; }
    public string? PhotoEnabled { get; set; }
    public string? PhotoDeleteCronCycle { get; set; }
    public string? PhotoDeleteOlderThan { get; set; }
    public string? DatabaseType { get; set; }
    public string? FFmpegPath { get; set; }
}
