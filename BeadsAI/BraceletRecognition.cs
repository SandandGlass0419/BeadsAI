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

        public static Dictionary<string, Rgba32[]> StrRgbMap => new()
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

        private static Rgba32[] RgbDefiner(string Path)
        {
            Image<Rgba32>[] images = BraceletRecognition.ProcessImage(Path);

            Rgba32[] colordef = images.Select(img => BraceletRecognition.GetAverageRGB(img)).ToArray();

            return colordef;
        }

        public static void PreLoadMap()
        {
            _ = StrRgbMap.Count;
        }
    }

    public class BraceletRecognition : RGBFindCore
    {
        public override Dictionary<string, Rgba32[]> StrRgbMap { get; protected set; } = BraceletRecognitionConfig.StrRgbMap;

        public override bool UseCieLab { get; protected set; } = BraceletRecognitionConfig.UseCieLab;

        public string[] FindBraceletColors(string Path)
        {
            Image<Rgba32>[] images = ProcessImage(Path);

            string[] BraceletColors = new string[BraceletRecognitionConfig.Parts];

            for (int pos = 0; pos < BraceletRecognitionConfig.Parts; pos++)
            {
                Rgba32 averageRgb = GetAverageRGB(images[pos]);
                BraceletColors[pos] = FindCloseColor(averageRgb, pos);
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
