using BeadsAI.Core.Color_Recognition;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace BeadsAI
{
    public class BraceletRecognition : RGBFindCore
    {
        public readonly Rectangle bounds = new(650,1950,3220,200);

        public const int parts = 32;

        // method to define bead colors, take image of one colored bracelet
        // and calculates average rgb value
        public Rgba32 RGBDefiner(string ColorName,string Path)
        {
            Image<Rgba32> image = BraceletRecognition.PathToImage(Path);

            image = CropTool.CropImage(image, bounds);

            Rgba32 new_rgb = GetAverageRGB(image);

            strRGBMap.Add(StringRGB.Create(ColorName,new_rgb));

            image.Dispose();

            return new_rgb;
        }

        public string[] FindBraceletColors(Image<Rgba32> image)
        {
            image = CropTool.CropImage(image,bounds);
            Image<Rgba32>[] images = CropTool.SplitImageVertical(image,parts);

            string[] BraceletColors = new string[0];

            foreach (var element_image in images)
            {
                Rgba32 averageRGB = GetAverageRGB(element_image);
                BraceletColors = [.. BraceletColors, FindCloseColor(averageRGB)];
            }

            image.Dispose();

            return BraceletColors;
        }
    }
}
