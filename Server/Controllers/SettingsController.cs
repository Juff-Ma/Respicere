namespace Respicere.Server.Controllers;

using Microsoft.AspNetCore.Mvc;
using Respicere.Server.Interfaces;
using Configuration = Shared.Configuration;

[Route("api/[controller]")]
[ApiController]
public class SettingsController : ControllerBase
{
    private readonly ILogger<SettingsController> _logger;
    private readonly IWritableOptions<Configuration> _configOptions;

    public SettingsController(ILogger<SettingsController> logger, IWritableOptions<Configuration> configOptions)
    {
        _logger = logger;
        _configOptions = configOptions;
    }

    [HttpGet]
    public Configuration Get()
    {
        return _configOptions.Value;
    }

    [HttpPut]
    public void Put(Configuration configuration)
    {
        _configOptions.Update(configuration);
        _logger.LogInformation("updated configuration, restart required");
    }
}
