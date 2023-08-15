using Microsoft.IdentityModel.Tokens;
using Respicere.Server.Interfaces;

namespace Respicere.Server;

public class PreprocessorWrapper<T> : IDataProcessor, IDataProducer<T> where T : IDataPreprocessor
{
    private readonly List<IDataProcessor> subscribers = new();
    private TaskCompletionSource _completion = new();

    public event EventHandler? Done;

    private readonly IDataProducer<IDataProcessor> _producer;

    public PreprocessorWrapper(IDataProducer<IDataProcessor> producer)
    {
        _producer = producer;
    }

    public async Task ProvideDataAsync(object data)
    {
        var preprocessedData = await T.PreprocessDataAsync(data);
        foreach (var processor in subscribers)
        {
            await processor.ProvideDataAsync(preprocessedData);
        }
    }

    public async Task RegisterDataProcessorAsync(T processor)
    {
        if (_completion.Task.IsCompleted)
        {
            _completion = new();
            await _producer.RegisterDataProcessorAsync(this);
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

        if (!subscribers.Any())
        {
            _completion.SetResult();
            Done?.Invoke(this, EventArgs.Empty);
        }
    }

    public Task WaitAsync(TimeSpan? timeout = null)
    {
        if (timeout is not null)
        {
            return Task.WhenAny(Task.Delay(timeout.Value), _completion.Task);
        }

        return _completion.Task;
    }
}
