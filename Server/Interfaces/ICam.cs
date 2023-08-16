using FlashCap;

namespace Respicere.Server.Interfaces;
public interface ICam
{
    string CamName { get; }
    VideoCharacteristics[] VideoCharacteristics { get; }

    /// <summary>
    /// Creates the video device
    /// </summary>
    Task CreateDeviceAsync();
    /// <summary>
    /// Takes a snapshot of the current camera image (only use when camera is not running)
    /// </summary>
    /// <param name="stream">The stream where the image (as jpeg) should be written to</param>
    Task TakeSnapshotAsync(Stream stream);
}