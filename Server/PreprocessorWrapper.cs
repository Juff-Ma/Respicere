using Respicere.Server.Interfaces;

namespace Respicere.Server;

public class PreprocessorWrapper<T> : IDataProcessor, IDataProducer where T : IDataPreprocessor
{
    private readonly List<IDataProcessor> subscribers = new();

    public void ProvideData(byte[] data)
    {
        var preprocessedData = T.PreprocessData(data);
        foreach (var processor in subscribers)
        {
            processor.ProvideData(preprocessedData);
        }
    }

    public void RegisterDataProcessor(IDataProcessor processor)
    {
        if (processor is T)
        {
            subscribers.Add(processor);
        }
        else
        {
            throw new ArgumentException("Subscribed data processor has to be equal to preprocessor type");
        }
    }
}
