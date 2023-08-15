// Ignore Spelling: mjpeg

namespace Respicere.Server.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Text;
using Interfaces;
using Helpers;
using Shared.Models;
using Respicere.Server.Processors;

[ApiController]
[Route("api/[controller]")]
public class VideoController : ControllerBase
{
    private readonly ICam _cam;
    private readonly PreprocessorWrapper<MjpegProcessor> _mjpegPreprocessor;

    public VideoController(ICam cam, PreprocessorWrapper<MjpegProcessor> mjpegPreprocessor)
    {
        _cam = cam;
        _mjpegPreprocessor = mjpegPreprocessor;
    }

    [HttpGet("cam")]
    public string GetCamName()
    {
        return _cam.CamName;
    }

    [HttpGet("cam/modes")]
    public async IAsyncEnumerable<VideoMode> GetPossibleModes()
    {
        var modes = _cam.GetVideoCharacteristics()
                            .Where(x => x.PixelFormat != FlashCap.PixelFormats.Unknown)
                            .Distinct(new DistinctCharacteristicsComparer())
                            .OrderByDescending(x => (x.Width * x.Height) + x.FramesPerSecond)
                            .ToAsyncEnumerable();
        await foreach (var mode in modes)
        {
            yield return new() { Width =  mode.Width, Height = mode.Height,
                                            FpsNumerator = mode.FramesPerSecond.Numerator,
                                            FpsDenominator = mode.FramesPerSecond.Denominator};
        }
    }

    [HttpGet("cam/mjpeg")]
    public async Task Get()
    {
        Response.StatusCode = 206;
        Response.ContentType = "multipart/x-mixed-replace; boundary=frame";
        Response.Headers.Add("Connection", "Keep-Alive");

        MjpegProcessor session = new(async data =>
        {
            if (Request.HttpContext.RequestAborted.IsCancellationRequested)
            {
                return;
            }

            await Response.BodyWriter.WriteAsync(CreateHeader(data.Length));
            await Response.BodyWriter.WriteAsync(data);
            await Response.BodyWriter.WriteAsync(CreateFooter());
            await Response.BodyWriter.FlushAsync();
        });

        await _mjpegPreprocessor.RegisterDataProcessorAsync(session);

        await Response.StartAsync();

        await session.WaitAsync();
    }

    /// <summary>
    /// Create an appropriate header.
    /// </summary>
    /// <param name="length"></param>
    /// <returns>the header</returns>
    private static byte[] CreateHeader(int length)
    {
        string header =
            "--frame" + "\r\n" +
            "Content-Type:image/jpeg\r\n" +
            "Content-Length:" + length + "\r\n\r\n";

        return Encoding.ASCII.GetBytes(header);
    }

    private static byte[] CreateFooter()
    {
        return Encoding.ASCII.GetBytes("\r\n");
    }
}
