// Ignore Spelling: Cron

namespace Respicere.Shared;

public class Configuration
{
    public string? CameraResolution { get; set; }
    public string? CameraFps { get; set;}
    public string? PhotoPath { get; set;}
    public string? PhotoTakeCronCycle { get; set; }
    public string? PhotoEnabled { get; set; }
    public string? PhotoDeleteCronCycle { get; set; }
    public string? PhotoDeleteOlderThan { get; set; }
}
