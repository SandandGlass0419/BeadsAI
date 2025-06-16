using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using BeadsAI;

namespace BeadsAI.Core.ColorRecognition
{
    public interface IRGBFindConfig
    {
        public static abstract Rectangle Bounds { get; }
        public static abstract int Parts { get; }
        public static Dictionary<string, Rgba32> strRGBMap { get; }
    }

    public abstract class RGBFindCore
    {
        public abstract Dictionary<string,Rgba32> strRGBMap { get; protected set; }

        public static Image<Rgba32> PathToImage(string path)
        {
            return Image.Load<Rgba32>(path);
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

        public string FindCloseColor(Rgba32 rgb)
        {
            (string color_name, double distance) minimum = ("None", float.MaxValue);

            foreach (string StringColor in strRGBMap.Keys) // candidate color
            {
                double distance = Math.Sqrt(Math.Pow(strRGBMap[StringColor].R - rgb.R,2) +
                                            Math.Pow(strRGBMap[StringColor].G - rgb.G,2) +
                                            Math.Pow(strRGBMap[StringColor].B - rgb.B,2));

                minimum = distance < minimum.distance ? (StringColor, distance) : minimum;
            }

            return minimum.color_name;
        }
    }
}
