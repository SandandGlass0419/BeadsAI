using System.Windows.Media;

namespace BeadsAI.UserControls
{
    public class UIBead
    {
        public int Position { get; protected set; }
        public string ColorName { get; protected set; }
        public SolidColorBrush Color { get; protected set; }

        public UIBead(int beadPosition, string beadColorName)
        {
            Position = beadPosition;
            ColorName = beadColorName;
            Color = new(ToColor(beadColorName));
        }

        public static Dictionary<string, byte[]> StrColorMap { get; } = new()
        {
            {"Red", [255,0,0] },
            {"Blue", [0,0,255] },
            {"Green", [0,255,0] },
            {"LightGreen", [173,225,47] }, // gr2
            {"SkyBlue", [135,205,235] }, // ls
            {"LightYellow", [225,225,191] }, // ly
            {"LightPink", [225,182,193] }, // p1
            {"Purple", [106,13,173] } // pp
        };

        public static UIBead[] ToBeads(string[] strBracelet)
        {
            return strBracelet.Select((name, i) => new UIBead(i, name)).ToArray();
        }

        private Color ToColor(string ColorName)
        {
            Color color = new();

            byte[]? rgb = [0, 0, 0];

            if (StrColorMap.TryGetValue(ColorName, out rgb))
            {
                color.A = 255;
                color.R = rgb[0];
                color.G = rgb[1];
                color.B = rgb[2];
            }

            return color;
        }

        public static string[] ToArray(UIBead[] Bracelet)
        {
            return Bracelet.Select(bead => bead.ColorName).ToArray();
        }
    }
}
