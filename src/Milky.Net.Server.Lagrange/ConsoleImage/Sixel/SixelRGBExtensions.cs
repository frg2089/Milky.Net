using System.Diagnostics;

namespace Milky.Net.Server.Lagrange.ConsoleImage.Sixel;

/// <summary>
/// Sixel RGB 颜色扩展
/// </summary>
public static class SixelRGBExtensions
{
    /// <inheritdoc cref="SixelWriter.RegistColor(byte, SixelColorType, int, int, int)"/>
    public static SixelWriter RegisterColor(this SixelWriter sixel, byte index, int r, int g, int b)
    {
        r = r * 100 / 255;
        g = g * 100 / 255;
        b = b * 100 / 255;

        return sixel.RegistColor(index, SixelColorType.RGB, r, g, b);
    }

    /// <inheritdoc cref="SixelWriter.RegistColor(byte, SixelColorType, int, int, int)"/>
    public static SixelWriter RegisterColor(this SixelWriter sixel, byte index, int rgb)
    {
        int r = (rgb & 0xFF0000) >> 0x10;
        int g = (rgb & 0x00FF00) >> 0x08;
        int b = (rgb & 0x0000FF) >> 0x00;

        return sixel.RegisterColor(index, r, g, b);
    }

    /// <inheritdoc cref="SixelWriter.RegistColor(byte, SixelColorType, int, int, int)"/>
    public static SixelWriter RegisterColor(this SixelWriter sixel, byte index, double r, double g, double b)
    {
        Debug.Assert(r is >= 0 and <= 1);
        Debug.Assert(g is >= 0 and <= 1);
        Debug.Assert(b is >= 0 and <= 1);

        return sixel.RegistColor(index, SixelColorType.RGB, (int)Math.Round(r * 100), (int)Math.Round(g * 100), (int)Math.Round(b * 100));
    }
}
