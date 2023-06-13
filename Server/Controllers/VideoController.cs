namespace Respicere.Server.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Text;
using Interfaces;

[ApiController]
[Route("api/[controller]")]
public class VideoController : ControllerBase
{
    private readonly ICam _cam;

    public VideoController(ICam cam)
    {
        _cam = cam;
    }

    [HttpGet]
    [Route("cam/mjpeg")]
    public async Task Get()
    {
        Response.StatusCode = 206;
        Response.ContentType = "multipart/x-mixed-replace; boundary=frame";
        Response.Headers.Add("Connection", "Keep-Alive");

        StreamingSession session = await _cam.StreamOnAsync(async data =>
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

        await Response.StartAsync();

        await session.WaitAsync();
    }

    /// <summary>
    /// Create an appropriate header.
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
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
