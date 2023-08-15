namespace Respicere.Server.Interfaces;

public interface IDataProducer<in T> where T : IDataProcessor
{
    Task RegisterDataProcessorAsync(T processor);
}
