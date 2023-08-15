// Ignore Spelling: Preprocess Mjpeg

using Respicere.Server.Interfaces;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace Respicere.Server.Processors;

public class MjpegProcessor : IDataPreprocessor
{
    public event EventHandler? Done;

    private readonly TaskCompletionSource _completion = new();
    private readonly Action<byte[]> _callback;

    public MjpegProcessor(Action<byte[]> callback)
    {
        _callback = callback;
    }

    public static async Task<object> PreprocessDataAsync(object data)
    {
        if (data is Image<Bgr24>)
        {
            using var ms = new MemoryStream();
            await (data as Image<Bgr24>).SaveAsJpegAsync(ms);

            return ms.ToArray();
        }
        else
        {
            throw new ArgumentException("Data for this preprocessor has to be Image with Pixel type Bgr24");
        }
    }

    public Task ProvideDataAsync(object data)
    {
        try
        {
            _callback((data as byte[])!);
        }
        catch (Exception)
        {
            _completion.SetResult();
            Done?.Invoke(this, EventArgs.Empty);
        }

        return Task.CompletedTask;
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
