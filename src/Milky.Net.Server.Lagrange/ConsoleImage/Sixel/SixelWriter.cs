using System.Diagnostics;

namespace Milky.Net.Server.Lagrange.ConsoleImage.Sixel;

/// <summary>
/// Sixel 工具
/// </summary>
/// <param name="writer"></param>
/// <param name="leaveOpen"></param>
public sealed class SixelWriter(TextWriter writer, bool leaveOpen = false) : IDisposable
{
    private const string DCS = "\eP";
    private const char DECGNL = '-';
    private const char DECGCR = '$';
    private const string ST = "\e\\";

    private bool _isBeginning;
    private int? _latestIndex;
    private int _repeatCounter;
    private int _line;
    private char? _latestSixel;
    private bool _disposedValue;

    /// <summary>
    /// 开始绘制 Sixel 图像
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public SixelWriter Begin(int width, int height)
    {
        Debug.Assert(!_isBeginning);
        writer.Write($"{DCS}0;1q\"1;1;{width};{height}");
        _latestIndex = null;
        _repeatCounter = 0;
        _line = 0;
        _latestSixel = null;

        _isBeginning = true;
        return this;
    }

    /// <summary>
    /// 注册颜色
    /// </summary>
    /// <param name="index"></param>
    /// <param name="colorType"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public SixelWriter RegistColor(byte index, SixelColorType colorType, int x, int y, int z)
    {
        switch (colorType)
        {
            case SixelColorType.HLS:
                if (x is < 0 or > 360)
                    throw new InvalidDataException();
                if (y is < 0 or > 100)
                    throw new InvalidDataException();
                if (z is < 0 or > 100)
                    throw new InvalidDataException();
                break;
            case SixelColorType.RGB:
                if (x is < 0 or > 100)
                    throw new InvalidDataException();
                if (y is < 0 or > 100)
                    throw new InvalidDataException();
                if (z is < 0 or > 100)
                    throw new InvalidDataException();
                break;
            default:
                throw new NotSupportedException();
        }

        _latestIndex = null;
        Debug.Assert(_isBeginning);
        writer.Write($"#{index};{(int)colorType};{x};{y};{z}");
        return this;
    }

    /// <summary>
    /// 写入像素
    /// </summary>
    /// <param name="colorIndex"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public SixelWriter WritePixel(byte? colorIndex, int count = 1)
    {
        Debug.Assert(_isBeginning);

        char sixel;
        if (colorIndex is null)
        {
            colorIndex = 0;
            sixel = '?';
        }
        else
        {
            sixel = (char)('?' + (1 << _line % 6));
        }

        if (_latestIndex != colorIndex)
        {
            if (_latestIndex is not null)
                Flush();
            writer.Write($"#{colorIndex}");
        }
        _latestIndex = colorIndex;

        if (_latestSixel is not null && _latestSixel != sixel)
        {
            switch (_repeatCounter)
            {
                case <= 3:
                    writer.Write(new string(_latestSixel.Value, _repeatCounter));
                    break;
                default:
                    writer.Write($"!{_repeatCounter}{_latestSixel}");
                    break;
            }

            _repeatCounter = 0;
        }
        _repeatCounter += count;
        _latestSixel = sixel;

        return this;
    }

    /// <summary>
    /// 立刻写入
    /// </summary>
    /// <returns></returns>
    public SixelWriter Flush()
    {
        Debug.Assert(_isBeginning);

        switch (_repeatCounter)
        {
            case <= 3:
                for (int i = 0; i < _repeatCounter; i++)
                    writer.Write($"{_latestSixel}");
                break;
            default:
                writer.Write($"!{_repeatCounter}{_latestSixel}");
                break;
        }
        _repeatCounter = 0;


        return this;
    }

    /// <summary>
    /// 下移一行
    /// </summary>
    /// <returns></returns>
    public SixelWriter NewLine(bool forceNewLine = false)
    {
        Debug.Assert(_isBeginning);
        Flush();
        writer.Write(DECGCR);
        if (forceNewLine || _line % 6 == 5)
            writer.Write(DECGNL);

        _line++;
        return this;
    }

    /// <summary>
    /// 停止写入
    /// </summary>
    /// <returns></returns>
    public SixelWriter End(bool newLine = true)
    {
        Debug.Assert(_isBeginning);
        Flush();

        if (newLine)
            writer.Write(DECGNL);
        writer.Write(ST);
        writer.Flush();
        _isBeginning = false;
        return this;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (_isBeginning)
                    End();

                if (!leaveOpen)
                    writer.Dispose();
            }

            _disposedValue = true;
        }
    }

    // ~Sixel()
    // {
    //     Dispose(disposing: false);
    // }


    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
    }
}
