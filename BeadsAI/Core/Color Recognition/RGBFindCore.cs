using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BeadsAI.Core.Color_Recognition
{
    public class RGBFindCore
    {
        public StringRGBMap strRGBMap { get; private set; } = new();

        public static Image<Rgba32> PathToImage(string path)
        {
            return Image.Load<Rgba32>(path);
        }

        protected Rgba32 GetAverageRGB(Image<Rgba32> image)
        {
            int pixelAmount = image.Width * image.Height;

            double redSum = SumRed(image);
            double greenSum = SumGreen(image);
            double blueSum = SumBlue(image);

            byte redAvg = (byte) (redSum / pixelAmount);
            byte greenAvg = (byte) (greenSum / pixelAmount);
            byte blueAvg = (byte) (blueSum / pixelAmount);

            return new Rgba32(redAvg, greenAvg, blueAvg);
        }

        private double SumRed(Image<Rgba32> image)
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

        private double SumGreen(Image<Rgba32> image)
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

        private double SumBlue(Image<Rgba32> image)
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

        protected string FindCloseColor(Rgba32 rgb)
        {
            (string color_name, double distance) minimum = ("None", float.MaxValue);

            foreach (var c_color in strRGBMap.RGBMap.Keys) // candidate color
            {
                double distance = Math.Sqrt(Math.Pow(strRGBMap.RGBMap[c_color].R - rgb.R,2) +
                                            Math.Pow(strRGBMap.RGBMap[c_color].G - rgb.G,2) +
                                            Math.Pow(strRGBMap.RGBMap[c_color].B - rgb.B,2));

                minimum = distance < minimum.distance ? (c_color, distance) : minimum;
            }

            return minimum.color_name;
        }
    }
}
