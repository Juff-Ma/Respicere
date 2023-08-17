using FFMediaToolkit.Encoding;

namespace Respicere.Server.Interfaces;

public interface IVideoWriter : IDataProcessor, IDisposable
{
    VideoEncoderSettings EncoderSettings { get; }

    public Task StartWrite(string path);
    public Task StopWrite();
}
