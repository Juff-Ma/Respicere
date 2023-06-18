// Ignore Spelling: Cron

namespace Respicere.Client.Helpers;

using Respicere.Shared;
using Respicere.Shared.Models;

public class ConfigurationWrapper
{
    private readonly Configuration _configuration;

    public VideoMode CurrentVideoMode { get; set; }
    public IEnumerable<VideoMode> VideoModes { get; }

    public ConfigurationWrapper(Configuration configuration, IEnumerable<VideoMode> videoModes)
    {
        _configuration = configuration;
        VideoModes = videoModes;

        CurrentVideoMode = videoModes.First(x =>
            x.Width == int.Parse(_configuration.CameraResolution?.Split('x')[0] ?? "1280") &&
            x.Height == int.Parse(_configuration.CameraResolution?.Split('x')[1] ?? "720") &&
            x.FpsNumerator == int.Parse(_configuration.CameraFpsNumerator ?? "30") &&
            x.FpsDenominator == int.Parse(_configuration.CameraFpsDenominator ?? "1")
        );
    }

    public bool PhotoEnabled
    {
        get
        {
            return bool.Parse(_configuration.PhotoEnabled ?? "false");
        }

        set
        {
            _configuration.PhotoEnabled = value.ToString();
        }
    }
    public string PhotoPath
    {
        get
        {
            return _configuration.PhotoPath ?? "./photos";
        }
        set
        {
            _configuration.PhotoPath = value;
        }
    }
    public string PhotoTakeCronCycle
    {
        get
        {
            return _configuration.PhotoTakeCronCycle ?? "0/10 * * ? * * *";
        }
        set
        {
            _configuration.PhotoTakeCronCycle = value;
        }
    }
    public string PhotoDeleteCronCycle
    {
        get
        {
            return _configuration.PhotoDeleteCronCycle ?? "0 0 0 ? * 1/3 *";
        }

        set
        {
            _configuration.PhotoDeleteCronCycle = value;
        }
    }
    public string PhotoDeleteOlderThan
    {
        get
        {
            return _configuration.PhotoDeleteOlderThan ?? "2.00:00";
        }
        set
        {
            _configuration.PhotoDeleteOlderThan = value;
        }
    }

    public Configuration ToConfiguration()
    {
        _configuration.CameraResolution = $"{CurrentVideoMode.Width}x{CurrentVideoMode.Height}";
        _configuration.CameraFpsNumerator = CurrentVideoMode.FpsNumerator.ToString();
        _configuration.CameraFpsDenominator = CurrentVideoMode.FpsDenominator.ToString();

        return _configuration;
    }
}
