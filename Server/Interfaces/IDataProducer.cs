namespace Respicere.Server.Interfaces;

public interface IDataProducer
{
    void RegisterDataProcessor(IDataProcessor processor);
}
