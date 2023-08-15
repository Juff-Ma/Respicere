namespace Respicere.Server;

using FlashCap;
using Respicere.Server.Helpers;
using Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.CompilerServices;

public class Cam : ICam, IDataProducer<IDataProcessor>
{
    private bool _signaledToStop;
    private readonly List<IDataProcessor> _processors = new();
    private readonly VideoCharacteristics _videoCharacteristics;
    private CaptureDevice? _captureDevice;
    private readonly CaptureDeviceDescriptor _captureDeviceDescriptor;

    public VideoCharacteristics[] VideoCharacteristics { get { return _captureDeviceDescriptor.Characteristics; } }
    public string CamName { get { return _captureDeviceDescriptor.Name; } }
    public Image<Bgr24>? LastFrame { get; private set; }

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
        byte[] data = bufferScope.Buffer.ExtractImage();
        LastFrame = Image.Load<Bgr24>(data);

        foreach (var processor in _processors.ToList())
        {
            await processor.ProvideDataAsync(LastFrame);
        }
    }

    public async Task TakeSnapshotAsync(Stream stream)
    {
        if (((!_captureDevice?.IsRunning) ?? false) || LastFrame is null)
        {
            var data = await _captureDeviceDescriptor.TakeOneShotAsync(_videoCharacteristics);
            LastFrame = Image.Load<Bgr24>(data);
        }

        await LastFrame.SaveAsJpegAsync(stream);
    }

    public VideoCharacteristics[] GetVideoCharacteristics()
    {
        return VideoCharacteristics;
    }

    public async Task RegisterDataProcessorAsync(IDataProcessor processor)
    {
        _processors.Add(processor);

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

        processor.Done += Processor_Done;
    }

    private async void Processor_Done(object? sender, EventArgs e)
    {
        _processors.Remove((sender as IDataProcessor)!);

        if (!_processors.Any())
        {
            await _captureDevice!.StopAsync();
            LastFrame = null;
            _signaledToStop = true;
        }
    }
}
