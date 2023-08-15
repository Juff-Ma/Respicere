using FlashCap;

namespace Respicere.Server.Interfaces;
public interface ICam
{
    string CamName { get; }

    Task CreateDeviceAsync();
    Task TakeSnapshotAsync(Stream stream);
    VideoCharacteristics[] GetVideoCharacteristics();
}