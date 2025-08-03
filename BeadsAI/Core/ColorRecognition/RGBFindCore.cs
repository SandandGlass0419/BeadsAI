using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;

namespace BeadsAI.Core.ColorRecognition
{
    public interface IRGBFindConfig
    {
        public static abstract Rectangle VerticalBounds { get; }
        public static abstract Rectangle HorizontalBounds { get; }

        public static abstract int Parts { get; }

        public static abstract Dictionary<string, Rgba32[]> StrRgbMap { get; }

        public static abstract bool UseCieLab { get; }
    }

    public abstract class RGBFindCore
    {
        public abstract Dictionary<string, Rgba32[]> StrRgbMap { get; protected set; }

        public abstract bool UseCieLab { get; protected set; }

        public static CieLab ToCieLab(Rgba32 rgba32)
        {
            ColorSpaceConverter converter = new();

            return converter.ToCieLab(rgba32);
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

        public string FindCloseColor(Rgba32 rgb, int pos) // no negative pos on real usage
        {
            (string color_name, double distance) minimum = ("None", double.MaxValue);

            foreach (string StringColor in StrRgbMap.Keys) // candidate color
            {
                double distance = UseCieLab ? GetLabDistance(ToCieLab(rgb), StringColor, pos)
                                            : GetRgbDistance(rgb, StringColor, pos);

                minimum = distance < minimum.distance ? (StringColor, distance) : minimum;
            }

            return minimum.color_name;
        }

        private double GetRgbDistance(Rgba32 rgb, string StringColor, int pos)
        {
            Rgba32 defedrgb = pos >= 0 ? StrRgbMap[StringColor][pos] : StrRgbAvgMap[StringColor];

            return
            Math.Sqrt(Math.Pow(defedrgb.R - rgb.R, 2) +
                      Math.Pow(defedrgb.G - rgb.G, 2) +
                      Math.Pow(defedrgb.B - rgb.B, 2));
        }

        private double GetLabDistance(CieLab lab, string StringColor, int pos)
        {
            Rgba32 defedrgb = pos >= 0 ? StrRgbMap[StringColor][pos] : StrRgbAvgMap[StringColor];
            CieLab defedlab = ToCieLab(defedrgb);

            return
            Math.Sqrt(Math.Pow(defedlab.L - lab.L, 2) +
                      Math.Pow(defedlab.A - lab.A, 2) +
                      Math.Pow(defedlab.B - lab.B, 2));
        }


        private static Dictionary<string, Rgba32> StrRgbAvgMap = new();

        public string FindCloseColor(Rgba32 rgb)
        {
            UpdateAvgMap();

            return FindCloseColor(rgb, -1);
        }

        private void UpdateAvgMap()
        {
            if (StrRgbAvgMap.Keys.ToArray() == StrRgbMap.Keys.ToArray())
            { return; }

            StrRgbAvgMap.Clear();

            foreach (var stringcolor in StrRgbMap.Keys)
            {
                StrRgbAvgMap.Add(stringcolor, GetMapValueAvg(stringcolor));
            }
        }

        private Rgba32 GetMapValueAvg(string stringcolor)
        {
            byte[] red = StrRgbMap[stringcolor].Select(rgb =>  rgb.R).ToArray();
            byte[] green = StrRgbMap[stringcolor].Select(rgb => rgb.G).ToArray();
            byte[] blue = StrRgbMap[stringcolor].Select(rgb => rgb.B).ToArray();

            byte avgred = (byte) red.Average(x => x);
            byte avggreen = (byte) green.Average(x => x);
            byte avgblue = (byte) blue.Average(x => x);

            return new Rgba32(avgred, avggreen, avgblue);
        }
    }
}
