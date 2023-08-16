namespace Respicere.Server.Interfaces;

public interface IDataProcessor
{
    /// <summary>
    /// Provides data to the processor to be processed
    /// </summary>
    /// <param name="data">The data to be processed.</param>
    Task ProvideDataAsync(object data);
    /// <summary>
    /// Waits until either the timeout has finished or all processing is done
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns></returns>
    Task WaitAsync(TimeSpan? timeout = null);

    /// <summary>
    /// Occurs when processing is done.
    /// </summary>
    event EventHandler? Done;
}
