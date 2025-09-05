using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace Milky.Net.Server.Lagrange.ConsoleImage.Sixel;

public static class Sixel
{
    public static string Process(Image image)
    {
        using Image<Rgba32> img = ((Func<Image<Rgba32>>)(() =>
        {
            var mw = 147;

            double weight = mw / image.Width;

            image.Mutate(ctx => ctx
                    .Resize((int)Math.Round(image.Width * weight), (int)Math.Round(image.Height * weight))
                    .Quantize(new WuQuantizer(new()
                    {
                        MaxColors = 256
                    })
                )
            );

            return image.CloneAs<Rgba32>();
        }))();

        using StringWriter writer = new();
        using SixelWriter sixel = new(writer);
        sixel.Begin(img.Width, img.Height);
        byte counter = 0;
        var palette = new Dictionary<Rgba32, byte>();
        byte?[,] indexes = new byte?[img.Width, img.Height];

        img.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (int x = 0; x < img.Width; x++)
                {
                    var pixel = row[x];
                    if (pixel.A is >= 128)
                    {
                        if (!palette.TryGetValue(pixel, out byte index))
                        {
                            index = counter++;
                            palette[pixel] = index;
                            //pixel.ToHsl(out var h, out var s, out var l);
                            //sixel.RegisterColorHSL(index, h, s, l);
                            sixel.RegisterColor(index, pixel.R, pixel.G, pixel.B);
                        }
                        indexes[x, y] = index;
                    }
                    else
                    {
                        indexes[x, y] = null;
                    }
                }
            }
        });

        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
                sixel.WritePixel(indexes[x, y]);

            sixel.NewLine();
        }

        sixel.End();

        return writer.ToString();
    }
}
