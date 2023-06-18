namespace Respicere.Shared.Helpers;

using Models;
using System.ComponentModel;
using System.Globalization;

public class VideoModeTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        var casted = value as string;
        var split = casted?.Split(',').Select(x => int.Parse(x)).ToArray();
        return split is not null
            ? new VideoMode()
            {
                Width = split[0],
                Height = split[1],
                FpsNumerator = split[2],
                FpsDenominator = split[3]
            }
            : base.ConvertFrom(context, culture, value);
    }
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        var casted = value as VideoMode;
        return destinationType == typeof(string) && casted is not null
            ? casted.ToString()
            : base.ConvertTo(context, culture, value, destinationType);
    }
}
