namespace Respicere.Server.Interfaces;

public interface IDataProducer<in T> where T : IDataProcessor
{
    void RegisterDataProcessor(T processor);
}
