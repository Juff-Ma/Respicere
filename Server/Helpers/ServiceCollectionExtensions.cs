namespace Respicere.Server.Helpers;

using Microsoft.Extensions.Options;
using Interfaces;

public static class ServiceCollectionExtensions
{
    public static void ConfigureWritable<T>(
        this IServiceCollection services,
        IConfigurationSection section,
        string file = "appsettings.json") where T : class, new()
    {
        services.Configure<T>(section);
        services.AddTransient<IWritableOptions<T>>(provider =>
        {
            var configuration = (IConfigurationRoot?)provider.GetService<IConfiguration>();
            var environment = provider.GetService<IWebHostEnvironment>();
            var options = provider.GetService<IOptionsMonitor<T>>();

            //if configuration doesn't exist bigger problems are there to handle
            return new WritableOptions<T>(environment!, options!, configuration!, section.Key, file);
        });
    }
}
