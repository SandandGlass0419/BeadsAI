using BeadsAI.Core.ColorRecognition;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace BeadsAI
{
    public class BraceletRecognitionConfig : IRGBFindConfig
    {
        public static Rectangle Bounds => new Rectangle(1,1,1,1);
        public static int Parts => 32;
         
        public const string saved_dir = "C:\\pics\\";

        public static Dictionary<string, Rgba32> strRGBMap => new()
        {
            {"Red",RGBDefiner(saved_dir + "Red.jpg") }
        };

        public static Rgba32 RGBDefiner(string Path)
        {
            Image<Rgba32> image = BraceletRecognition.PathToImage(Path);

            image = CropTool.CropImage(image, Bounds);

            Rgba32 new_rgb = BraceletRecognition.GetAverageRGB(image);

            image.Dispose();

            return new_rgb;
        }
    }

    public class BraceletRecognition : RGBFindCore
    {
        public override Dictionary<string, Rgba32> strRGBMap { get; protected set; } = BraceletRecognitionConfig.strRGBMap;

        public string[] FindBraceletColors(string Path)
        {
            Image<Rgba32>[] images = ProcessImage(Path);

            string[] BraceletColors = Array.Empty<string>();

            foreach (var element_image in images)
            {
                Rgba32 averageRGB = GetAverageRGB(element_image);
                BraceletColors = [.. BraceletColors, FindCloseColor(averageRGB)];
            }
            
            return BraceletColors;
        }

        private Image<Rgba32>[] ProcessImage(string Path)
        {
            Image<Rgba32> image = PathToImage(Path);

            image = CropTool.CropImage(image, BraceletRecognitionConfig.Bounds);

            Image<Rgba32>[] images = CropTool.SplitImageVertical(image, BraceletRecognitionConfig.Parts);

            image.Dispose();

            return images;
        }
    }
}
