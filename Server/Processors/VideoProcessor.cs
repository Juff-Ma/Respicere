// Ignore Spelling: Preprocess

using FFMediaToolkit.Encoding;
using Respicere.Server.Interfaces;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Runtime.CompilerServices;
using FFMediaToolkit.Graphics;

namespace Respicere.Server.Processors;

public class VideoProcessor : IDataPreprocessor
{
    public event EventHandler? Done;

    protected void OnDone(EventArgs e)
        => Done?.Invoke(this, e);

    protected TaskCompletionSource _completion = new();
    protected MediaOutput? _output;

    private readonly ILogger _logger;

    public VideoProcessor(ILogger logger)
    {
        _logger = logger;
    }

    public VideoProcessor(MediaOutput output, ILogger logger) : this(logger)
    {
        _output = output;
    }

    public static Task<object> PreprocessDataAsync(object data)
    {
        if (data is Image<Bgr24>)
        {
            var image = (data as Image<Bgr24>)!;
            byte[] buffer = new byte[image.Width * image.Height * Unsafe.SizeOf<Bgr24>()];

            image.CopyPixelDataTo(buffer);

            return Task.FromResult(
                new ImageBuffer(buffer)
                {
                    Format = ImagePixelFormat.Bgr24,
                    Width = image.Width,
                    Height = image.Height
                } as object);
        }
        else
        {
            throw new ArgumentException("Data for this preprocessor has to be Image with Pixel type Bgr24");
        }
    }

    public class ImageBuffer
    {
        readonly byte[] _buffer;

        public required ImagePixelFormat Format { get; init; }
        public required int Width { get; init; }
        public required int Height { get; init; }

        public byte this[int selector]
        {
            get
            {
                return _buffer[selector];
            }
            set
            {
                _buffer[selector] = value;
            }
        }

        public ImageBuffer(byte[] buffer)
        {
            _buffer = buffer;
        }

        public ImageData ToImageData()
        {
            return ImageData.FromArray(_buffer, Format, Width, Height);
        }

        public static implicit operator byte[](ImageBuffer buffer) => buffer._buffer;
    }

    public Task ProvideDataAsync(object data)
    {
        try
        {
            if (_output is not null && data is ImageBuffer)
            {
                var image = (data as ImageBuffer)!.ToImageData();

                _output?.Video.AddFrame(image);
                _logger.LogTrace("successfully wrote frame to video");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error writing frame to video file");
            _completion.SetResult();

            _output?.Dispose();

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
