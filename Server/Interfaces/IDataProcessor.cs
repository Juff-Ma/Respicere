namespace Respicere.Server.Interfaces;

public interface IDataProcessor
{
    Task ProvideDataAsync(object data);
    Task WaitAsync(TimeSpan? timeout = null);

    event EventHandler? Done;
}
