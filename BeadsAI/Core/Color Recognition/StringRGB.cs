using SixLabors.ImageSharp.PixelFormats;

namespace BeadsAI.Core.Recognition
{
    public sealed class StringRGB
    {
        public string ColorName { get; private set; }
        public Rgba32 Rgb { get; private set; }

        public StringRGB(string ColorName, Rgba32 Rgb)
        {
            this.ColorName = ColorName;
            this.Rgb = Rgb;
        }

        public static StringRGB Create(string ColorName, Rgba32 Rgb)
        {
            return new StringRGB(ColorName, Rgb);
        }

        public static Rgba32 ToRgba32(double R, double G, double B) // 0 ~ 1
        {
            return new Rgba32((byte) R, (byte) G, (byte) B);
        }
    }

    public sealed class StringRGBMap
    {
        public Dictionary<string, Rgba32> RGBMap { get; private set; } = new();

        public void Add(StringRGB srgb)
        {
            StringRGBMetRequirments(srgb);

            RGBMap.Add(srgb.ColorName, srgb.Rgb);
        }

        private bool StringRGBMetRequirments(StringRGB srgb)
        {
            return true;
        }
    }
}
