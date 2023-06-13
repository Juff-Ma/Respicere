namespace Respicere.Server;

public class StreamingSession
{
    public StreamingSession(Action<byte[]> Callback)
    {
        this.Callback = Callback;
    }

    private readonly Action<byte[]> Callback;
    private readonly TaskCompletionSource Completion = new();

    public delegate void SessionEndedEventHandler(StreamingSession sender, EventArgs args);
    public event SessionEndedEventHandler? OnSessionEnded;

    public Task WaitAsync(int? timeout = null)
    {
        if (timeout.HasValue)
        {
            return Task.WhenAny(Task.Delay(timeout.Value), Completion.Task);
        }

        return Completion.Task;
    }

    public void ProvideData(byte[] data)
    {
        try
        {
            Callback(data);
        }
        catch (Exception)
        {
            EndSession();
        }
    }

    public void EndSession()
    {
        Completion.SetResult();
        OnSessionEnded?.Invoke(this, EventArgs.Empty);
    }
}
