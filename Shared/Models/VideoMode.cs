namespace Respicere.Shared.Models;

using Helpers;
using System.ComponentModel;

[TypeConverter(typeof(VideoModeTypeConverter))]
public class VideoMode
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int FpsNumerator { get; set; }
    public int FpsDenominator { get; set; }

    public string ToString(bool decorated)
    {
        if (decorated)
        {
            return $"{Width}x{Height}, {FpsNumerator / FpsDenominator:F4} FPS";
        }
        else
        {
            return ToString();
        }
    }

    public override string ToString()
    {
        return $"{Width},{Height},{FpsNumerator},{FpsDenominator}";
    }
}
