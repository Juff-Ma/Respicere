using Microsoft.Extensions.Options;

namespace Respicere.Server.Interfaces;
public interface IWritableOptions<T> : IOptions<T> where T : class, new()
{
    void Update(Action<T> applyChanges);
    void Update(T sectionObject);
}
