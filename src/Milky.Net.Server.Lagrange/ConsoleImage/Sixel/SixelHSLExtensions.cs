using System.Diagnostics;

namespace Milky.Net.Server.Lagrange.ConsoleImage.Sixel;

/// <summary>
/// Sixel HSL 颜色扩展
/// </summary>
public static class SixelHSLExtensions
{
    /// <inheritdoc cref="SixelWriter.RegistColor(byte, SixelColorType, int, int, int)"/>
    public static SixelWriter RegistColorHSL(this SixelWriter sixel, byte index, int h, int s, int l)
    {
        Debug.Assert(h is >= 0 and <= 360);
        Debug.Assert(l is >= 0 and <= 100);
        Debug.Assert(s is >= 0 and <= 100);

        return sixel.RegistColor(index, SixelColorType.HLS, h, l, s);
    }

    /// <inheritdoc cref="SixelWriter.RegistColor(byte, SixelColorType, int, int, int)"/>
    public static SixelWriter RegistColorHSL(this SixelWriter sixel, byte index, float h, float s, float l)
    {
        Debug.Assert(h is >= 0 and <= 360);
        Debug.Assert(l is >= 0 and <= 100);
        Debug.Assert(s is >= 0 and <= 100);

        return sixel.RegistColorHSL(index, (int)Math.Round(h), (int)Math.Round(s), (int)Math.Round(l));
    }
}
