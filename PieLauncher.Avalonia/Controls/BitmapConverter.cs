using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using ABitmap = Avalonia.Media.Imaging.Bitmap;
using PixelFormat = Avalonia.Platform.PixelFormat;

namespace PieLauncher.Avalonia.Controls;

public class BitmapConverter : MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not BitmapSource bitmap) return null;

        var stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
        var bytes = new byte[bitmap.PixelHeight * stride];
        bitmap.CopyPixels(bytes, stride, 0);

        var ptr = Marshal.AllocHGlobal(bytes.Length);

        Marshal.Copy(bytes, 0, ptr, bytes.Length);

        var ret = new ABitmap(PixelFormat.Bgra8888, AlphaFormat.Unpremul,
            ptr,
            new PixelSize(bitmap.PixelWidth, bitmap.PixelHeight),
            new Vector(96, 96),
            stride);

        Marshal.FreeHGlobal(ptr);

        return ret;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}