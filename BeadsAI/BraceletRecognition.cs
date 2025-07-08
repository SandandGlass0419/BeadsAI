using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using BeadsAI.Core.ColorRecognition;

namespace BeadsAI
{
    public class BraceletRecognitionConfig : IRGBFindConfig
    {
        public static Rectangle Bounds => new Rectangle(728,1965,3200,100);
        public static int Parts => 32;
         
        public const string saved_dir = "C:\\pics\\";

        public static Dictionary<string, CieLab> strLabMap => new()
        {
            {"Red" , LabDefiner(saved_dir + "Red.jpg") },
            {"Purple" , LabDefiner(saved_dir + "Purple.jpg") }
        };

        private static CieLab LabDefiner(string Path)
        {
            Image<Rgba32> image = BraceletRecognition.PathToImage(Path);

            image = CropTool.CropImage(image, Bounds);

            Rgba32 new_rgba32 = BraceletRecognition.GetAverageRGB(image);

            image.Dispose();

            return BraceletRecognition.ToCieLab(new_rgba32);
        }
    }

    public class BraceletRecognition : RGBFindCore
    {
        public override Dictionary<string, CieLab> strLabMap { get; protected set; } = BraceletRecognitionConfig.strLabMap;

        public string[] FindBraceletColors(string Path)
        {
            Image<Rgba32>[] images = ProcessImage(Path);

            string[] BraceletColors = Array.Empty<string>();

            foreach (var element_image in images)
            {
                Rgba32 averageRGB = GetAverageRGB(element_image);
                BraceletColors = [.. BraceletColors, FindCloseColor(ToCieLab(averageRGB))];
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
