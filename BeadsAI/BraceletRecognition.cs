using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using BeadsAI.Core.ColorRecognition;

namespace BeadsAI
{
    public class BraceletRecognitionConfig : IRGBFindConfig
    {
        public static Rectangle Bounds => new Rectangle(1510,550,150,2450);
        public static int Parts => 32;
         
        public const string saved_dir = "C:\\BeadsFolder\\ColorDefs\\";

        public static Dictionary<string, CieLab> StrLabMap => new()
        {
            {"Red", new CieLab(53.232f,80.109f,67.220f) },
            {"Blue", new CieLab(87.737f,-86.184f,83.181f) },
            {"Green", new CieLab(32.302f,79.196f,-107.863f) },
            {"LightGreen", new CieLab(91.957f,-52.483f,81.866f) },
            {"SkyBlue", new CieLab(90.299f,-11.507f,-14.875f) },
            {"LightYellow", new CieLab(90.871f,-16.658f,40.497f) },
            {"LightPink", new CieLab(81.052f,27.968f,5.025f) },
            {"Purple", new CieLab(68.119f,19.873f,-28.206f) }
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
        public override Dictionary<string, CieLab> StrLabMap { get; protected set; } = BraceletRecognitionConfig.StrLabMap;

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