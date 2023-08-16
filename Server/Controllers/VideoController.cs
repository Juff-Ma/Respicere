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
    private readonly IDataProducer<MjpegProcessor> _mjpegProducer;

    public VideoController(ICam cam, IDataProducer<MjpegProcessor> mjpegProducer)
    {
        _cam = cam;
        _mjpegProducer = mjpegProducer;
    }

    /// <summary>
    /// returns the cameras device name
    /// </summary>
    /// <returns>the camera name</returns>
    [HttpGet("cam")]
    public string GetCamName()
    {
        return _cam.CamName;
    }

    /// <summary>
    /// returns the video modes supported by the camera
    /// </summary>
    /// <returns>all possible modes</returns>
    [HttpGet("cam/modes")]
    public async IAsyncEnumerable<VideoMode> GetPossibleModes()
    {
        var modes = _cam.VideoCharacteristics
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

    /// <summary>
    /// returns a stream of the cameras live picture, is kept alive
    /// </summary>
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

        await _mjpegProducer.RegisterDataProcessorAsync(session);

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
            $"""
            --frame\r\n
            Content-Type:image/jpeg\r\n
            Content-Length:{length}\r\n\r\n
            """;

        return Encoding.ASCII.GetBytes(header);
    }

    /// <summary>
    /// Create an appropriate footer.
    /// </summary>
    /// <returns>the footer</returns>
    private static byte[] CreateFooter()
    {
        return Encoding.ASCII.GetBytes("\r\n");
    }
}
