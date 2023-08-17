using FFMediaToolkit.Encoding;
using Respicere.Server.Helpers;
using Respicere.Server.Interfaces;
using Respicere.Server.Processors;
using Respicere.Shared;

namespace Respicere.Server.Services;

public class VideoWriterService : VideoProcessor, IVideoWriter
{
    public VideoEncoderSettings EncoderSettings { get; private init; }

    private readonly IDataProducer<VideoProcessor> _producer;
    private readonly ILogger<VideoWriterService> _logger;

    public VideoWriterService(IDataProducer<VideoProcessor> producer, ILogger<VideoWriterService> logger, IWritableOptions<Configuration> options) : base(logger)
    {
        _producer = producer;
        _logger = logger;

        EncoderSettings = new(
            options.Value.GetWidth(),
            options.Value.GetHeight())
        {
            Codec = VideoCodec.H264,

            FramerateRational = new()
            {
                num = options.Value.GetFps().Numerator,
                den = options.Value.GetFps().Denominator
            },

            EncoderPreset = EncoderPreset.Medium,
            CRF = 25
        };
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _output?.Dispose();
    }

    public async Task StartWrite(string path)
    {
        _output = MediaBuilder.CreateContainer(path).WithVideo(EncoderSettings).Create();

        await _producer.RegisterDataProcessorAsync(this);

        _logger.LogInformation("started new video recording");
    }

    public Task StopWrite()
    {
        _output?.Dispose();
        _output = null;

        OnDone(EventArgs.Empty);

        _logger.LogInformation("successfully recorded video");

        return Task.CompletedTask;
    }
}
