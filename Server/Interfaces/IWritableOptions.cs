using Microsoft.Extensions.Options;

namespace Respicere.Server.Interfaces;
public interface IWritableOptions<T> : IOptions<T> where T : class, new()
{
    /// <summary>
    /// Updates the configuration.
    /// </summary>
    /// <param name="applyChanges">Action with changes to be applied</param>
    void Update(Action<T> applyChanges);
    /// <summary>
    /// Updates the configuration.
    /// </summary>
    /// <param name="sectionObject">The update that the configuration is to be replaced with</param>
    void Update(T sectionObject);
}
