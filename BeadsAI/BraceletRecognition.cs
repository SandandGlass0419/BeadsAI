using BeadsAI.Core;
using BeadsAI.Core.ColorRecognition;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace BeadsAI
{
    public class BraceletRecognitionConfig : IRGBFindConfig
    {
        public static Rectangle VerticalBounds => new Rectangle(0, 0, 0, 0);
        public static Rectangle HorizontalBounds => new Rectangle(0, 0, 0, 0);
        public static int Parts => 32;
        public static bool UseCieLab => false;
         
        public const string saved_dir = "C:\\BeadsFolder\\ColorDefs\\";

        public static Dictionary<string, Rgba32> StrRgbMap => new()
        {
            {"Red", RgbDefiner(saved_dir + "Red.jpg") },
            {"Blue", RgbDefiner(saved_dir + "Blue.jpg") },
            {"Green", RgbDefiner(saved_dir + "Green.jpg") },
            {"LightGreen", RgbDefiner(saved_dir + "LightGreen.jpg") },
            {"SkyBlue", RgbDefiner(saved_dir + "SkyBlue.jpg") },
            {"LightYellow", RgbDefiner(saved_dir + "LightYellow.jpg") },
            {"LightPink", RgbDefiner(saved_dir + "LightPink.jpg") },
            {"Purple", RgbDefiner(saved_dir + "Purple.jpg") }
        };

        private static Rgba32 RgbDefiner(string Path)
        {
            Image<Rgba32> image = BraceletRecognition.PathToImage(Path);

            Rectangle bounds = image.Width > image.Height ? HorizontalBounds : VerticalBounds;

            image = CropTool.CropImage(image, bounds);

            Rgba32 new_rgba32 = BraceletRecognition.GetAverageRGB(image);

            image.Dispose();

            return new_rgba32;
        }
    }

    public class BraceletRecognition : RGBFindCore
    {
        public override Dictionary<string, Rgba32> StrRgbMap { get; protected set; } = BraceletRecognitionConfig.StrRgbMap;
        public override bool UseCieLab { get; protected set; } = BraceletRecognitionConfig.UseCieLab;

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

        public static Image<Rgba32>[] ProcessImage(string Path)
        {
            Image<Rgba32> image = PathToImage(Path);
            Image<Rgba32>[] images = Array.Empty<Image<Rgba32>>();

            if (image.Width > image.Height) // prioritizes vertical when 1 : 1 image is input
            {
                image = CropTool.CropImage(image, BraceletRecognitionConfig.HorizontalBounds);
                images = CropTool.SplitHorizontalImage(image, BraceletRecognitionConfig.Parts);
            }

            else
            {
                image = CropTool.CropImage(image, BraceletRecognitionConfig.VerticalBounds);
                images = CropTool.SplitVerticalImage(image, BraceletRecognitionConfig.Parts);
            }

            image.Dispose();

            return images;
        }

        public static Image<Rgba32> PathToImage(string path)
        {
            if (!File.Exists(path))
            { ExceptionThrower.Throw($"File: {path} does not exist."); }

            return Image.Load<Rgba32>(path);
        }
    }
}
