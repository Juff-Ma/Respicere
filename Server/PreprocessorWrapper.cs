using Microsoft.IdentityModel.Tokens;
using Respicere.Server.Interfaces;

namespace Respicere.Server;

public class PreprocessorWrapper<T> : IDataProcessor, IDataProducer<T> where T : IDataPreprocessor
{
    private readonly List<IDataProcessor> subscribers = new();
    private TaskCompletionSource Completion = new();

    public event EventHandler? Done;

    private readonly IDataProducer<IDataProcessor> _producer;

    public PreprocessorWrapper(IDataProducer<IDataProcessor> producer)
    {
        _producer = producer;
    }

    public void ProvideData(object data)
    {
        var preprocessedData = T.PreprocessData(data);
        foreach (var processor in subscribers)
        {
            processor.ProvideData(preprocessedData);
        }
    }

    public void RegisterDataProcessor(T processor)
    {
        if (Completion.Task.IsCompleted)
        {
            Completion = new();
            _producer.RegisterDataProcessor(this);
        }
        subscribers.Add(processor);
        processor.Done += Subscriber_Done;
    }

    private void Subscriber_Done(object? sender, EventArgs e)
    {
        if(sender is T && subscribers.Contains(sender))
        {
            subscribers.Remove((sender as IDataProcessor)!);
        }

        if (subscribers.IsNullOrEmpty())
        {
            Completion.SetResult();
            Done?.Invoke(this, EventArgs.Empty);
        }
    }

    public Task WaitAsync(TimeSpan? timeout = null)
    {
        if (timeout is not null)
        {
            return Task.WhenAny(Task.Delay(timeout.Value), Completion.Task);
        }

        return Completion.Task;
    }
}
