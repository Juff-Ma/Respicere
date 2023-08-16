namespace Respicere.Server.Interfaces;

public interface IDataProducer<in T> where T : IDataProcessor
{
    /// <summary>
    /// Registers a data processor for this producer.
    /// </summary>
    /// <param name="processor">The processor to be registered.</param>
    Task RegisterDataProcessorAsync(T processor);
}
