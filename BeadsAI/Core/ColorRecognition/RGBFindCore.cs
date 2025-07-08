using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace BeadsAI.Core.ColorRecognition
{
    public interface IRGBFindConfig
    {
        public static abstract Rectangle Bounds { get; }
        public static abstract int Parts { get; }
        public static abstract Dictionary<string, CieLab> strLabMap { get; }
    }

    public abstract class RGBFindCore
    {
        public abstract Dictionary<string, CieLab> strLabMap { get; protected set; }

        public static Image<Rgba32> PathToImage(string path)
        {
            if (!File.Exists(path))
            { ExceptionThrower.Throw($"File: {path} does not exist."); }

            return Image.Load<Rgba32>(path);
        }

        public static CieLab ToCieLab(Rgba32 rgba32)
        {
            ColorSpaceConverter converter = new();

            var rgb = new Rgb(rgba32.R / 255f, rgba32.G / 255f, rgba32.B / 255f);

            return converter.ToCieLab(rgb);
        }

        public static Rgba32 GetAverageRGB(Image<Rgba32> image)
        {
            int pixelAmount = image.Width * image.Height;

            double redSum = SumRed(image);
            double greenSum = SumGreen(image);
            double blueSum = SumBlue(image);

            byte redAvg = (byte) (redSum / pixelAmount);
            byte greenAvg = (byte) (greenSum / pixelAmount);
            byte blueAvg = (byte) (blueSum / pixelAmount);

            image.Dispose();

            return new Rgba32(redAvg, greenAvg, blueAvg);
        }

        private static double SumRed(Image<Rgba32> image)
        {
            double red = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    red += image[x, y].R;
                }
            }

            return red;
        }

        private static double SumGreen(Image<Rgba32> image)
        {
            double green = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    green += image[x, y].G;
                }
            }

            return green;
        }

        private static double SumBlue(Image<Rgba32> image)
        {
            double blue = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    blue += image[x, y].B;
                }
            }

            return blue;
        }

        public string FindCloseColor(CieLab lab)
        {
            (string color_name, double distance) minimum = ("None", double.MaxValue);

            foreach (string StringColor in strLabMap.Keys) // candidate color
            {
                double distance = Math.Sqrt(Math.Pow(strLabMap[StringColor].L - lab.L,2) +
                                            Math.Pow(strLabMap[StringColor].A - lab.A,2) +
                                            Math.Pow(strLabMap[StringColor].B - lab.B,2));

                minimum = distance < minimum.distance ? (StringColor, distance) : minimum;
            }

            return minimum.color_name;
        }
    }
}
