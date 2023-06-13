using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Respicere.Server.Interfaces;

namespace Respicere.Server;

public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
{
    private readonly IWebHostEnvironment _environment;
    private readonly IOptionsMonitor<T> _options;
    private readonly IConfigurationRoot _configuration;
    private readonly string _section;
    private readonly string _file;

    public WritableOptions(
        IWebHostEnvironment environment,
        IOptionsMonitor<T> options,
        IConfigurationRoot configuration,
        string section,
        string file)
    {
        _environment = environment;
        _options = options;
        _configuration = configuration;
        _section = section;
        _file = file;
    }

    public T Value => _options.CurrentValue;
    public T Get(string name) => _options.Get(name);

    public void Update(T sectionObject)
    {
        var fileProvider = _environment.ContentRootFileProvider;
        var fileInfo = fileProvider.GetFileInfo(_file);
        var physicalPath = fileInfo.PhysicalPath ?? "";

        var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(physicalPath));

        if (jObject is not null)
            jObject[_section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
        File.WriteAllText(physicalPath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
        _configuration?.Reload();
    }

    public void Update(Action<T> applyChanges)
    {
        var fileProvider = _environment.ContentRootFileProvider;
        var fileInfo = fileProvider.GetFileInfo(_file);
        var physicalPath = fileInfo.PhysicalPath ?? "";

        var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(physicalPath));

        var sectionObject = jObject?.TryGetValue(_section, out JToken? section) ?? false ?
            JsonConvert.DeserializeObject<T>(section.ToString()) : (Value ?? new T());

        if (sectionObject is not null)
            applyChanges(sectionObject);

        if (jObject is not null)
            jObject[_section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
        File.WriteAllText(physicalPath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
        _configuration?.Reload();
    }
}