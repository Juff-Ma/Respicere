namespace Respicere.Server;

using FlashCap;
using Respicere.Server.Helpers;
using Interfaces;
using SixLabors.ImageSharp;

public class Cam : ICam
{
    private bool _signaledToStop = false;
    private readonly List<StreamingSession> _sessions = new();
    private readonly VideoCharacteristics _videoCharacteristics;
    private CaptureDevice? _captureDevice;
    private readonly CaptureDeviceDescriptor _captureDeviceDescriptor;

    public VideoCharacteristics[] VideoCharacteristics { get { return _captureDeviceDescriptor.Characteristics; } }
    public string CamName { get { return _captureDeviceDescriptor.Name; } }
    public Image? LastFrame { get; private set; }

    public Cam(CaptureDeviceDescriptor captureDeviceDescriptor, VideoCharacteristics videoCharacteristics)
    {
        _captureDeviceDescriptor = captureDeviceDescriptor;
        _videoCharacteristics = videoCharacteristics;
    }

    public async Task CreateDeviceAsync()
    {
        _captureDevice = await _captureDeviceDescriptor.OpenAsync(
            _videoCharacteristics, GotNewFrame);
    }

    private async Task GotNewFrame(PixelBufferScope bufferScope)
    {
        byte[] data;
        byte[] imageData = bufferScope.Buffer.ExtractImage();
        LastFrame = Image.Load(imageData);

        using (var stream = new MemoryStream())
        {
            await LastFrame.SaveAsJpegAsync(stream);

            data = stream.ToArray();
        }

        foreach (var session in _sessions.ToList())
        {
            session.ProvideData(data);
        }
    }

    public async Task<StreamingSession> StreamOnAsync(Action<byte[]> callback)
    {
        var session = new StreamingSession(callback);
        _sessions.Add(session);

        if (_captureDevice is null)
        {
            await CreateDeviceAsync();
        }
        if (_signaledToStop)
        {
            await Waiter.WaitUntil(() => !_captureDevice!.IsRunning);
        }
        if (!_captureDevice!.IsRunning)
        {
            await _captureDevice.StartAsync();
            _signaledToStop = false;
        }

        session.OnSessionEnded += Session_OnSessionEnded;

        return session;
    }

    private async void Session_OnSessionEnded(StreamingSession sender, EventArgs e)
    {
        _sessions.Remove(sender);

        if (!_sessions.Any())
        {
            await _captureDevice!.StopAsync();
            LastFrame = null;
            _signaledToStop = true;
        }
    }

    public async Task TakeSnapshotAsync(Stream stream)
    {
        if (((!_captureDevice?.IsRunning) ?? false) || LastFrame is null)
        {
            var imageData = await _captureDeviceDescriptor.TakeOneShotAsync(_videoCharacteristics);
            LastFrame = Image.Load(imageData);
        }

        await LastFrame.SaveAsJpegAsync(stream);
    }

    public VideoCharacteristics[] GetVideoCharacteristics()
    {
        return VideoCharacteristics;
    }

    public string GetCamName()
    {
        return CamName;
    }
}
