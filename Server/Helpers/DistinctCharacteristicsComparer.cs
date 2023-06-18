namespace Respicere.Server.Helpers;

using FlashCap;
using System.Diagnostics.CodeAnalysis;

public class DistinctCharacteristicsComparer : IEqualityComparer<VideoCharacteristics>
{
    public bool Equals(VideoCharacteristics? x, VideoCharacteristics? y)
    {
        if (x is null || y is null) return false;

        return x.Width == y.Width &&
            x.Height == y.Height &&
            x.FramesPerSecond == y.FramesPerSecond;
    }

    public int GetHashCode([DisallowNull] VideoCharacteristics obj)
    {
        return obj.Height.GetHashCode() ^ obj.Width.GetHashCode() ^ obj.FramesPerSecond.GetHashCode();
    }
}
