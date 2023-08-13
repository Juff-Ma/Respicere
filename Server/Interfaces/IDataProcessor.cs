namespace Respicere.Server.Interfaces;

public interface IDataProcessor
{
    void ProvideData(object data);
    Task WaitAsync(TimeSpan? timeout = null);

    event EventHandler? Done;
}
