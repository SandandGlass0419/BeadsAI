using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace BeadsAI.Core.Color_Recognition
{
    public class CropTool
    {
        public static Image<Rgba32> CropImage(Image<Rgba32> source,Rectangle bounds)
        {
            return source.Clone(ipc => ipc.Crop(bounds));  
        }

        public static Image<Rgba32>[] SplitImageHorizantal(Image<Rgba32> source,int parts) // vertical becomes shorter
        {
            int margin_px = source.Height / parts;
            Image<Rgba32>[] Splits = new Image<Rgba32>[parts];

            for (int i = 0; i < parts; i++)
            {
                Rectangle margin = new(0, margin_px * i, source.Width, margin_px);
                Splits[i] = CropImage(source, margin);
            }

            return Splits;
        }

        public static Image<Rgba32>[] SplitImageVertical(Image<Rgba32> source,int parts) // horizantal becomes shorter
        {
            int margin_px = source.Width / parts;
            Image<Rgba32>[] Splits = new Image<Rgba32>[parts];

            for (int i = 0; i < parts; i++)
            {
                Rectangle margin = new(margin_px * i, 0, margin_px, source.Height);
                Splits[i] = CropImage(source, margin);
            }

            return Splits;
        }
    }
}
