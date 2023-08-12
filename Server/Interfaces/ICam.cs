using FlashCap;

namespace Respicere.Server.Interfaces;
public interface ICam
{
    string CamName { get; }

    Task CreateDeviceAsync();
    Task<StreamingSession> StreamOnAsync(Action<byte[]> callback);
    Task TakeSnapshotAsync(Stream stream);
    VideoCharacteristics[] GetVideoCharacteristics();
}