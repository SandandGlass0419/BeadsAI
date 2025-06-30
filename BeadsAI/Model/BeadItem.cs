using System.Windows.Media;

namespace BeadsAI.Model
{
    public class BeadItem
    {
        public int BeadPosition { get; set; }
        public string BeadColorName { get; set; }
        public SolidColorBrush BeadColor { get; set; }

        public BeadItem(int beadPosition, string beadColorName)
        {
            BeadPosition = beadPosition;
            BeadColorName = beadColorName;
            BeadColor = new (ToColor(beadColorName));
        }

        public static Dictionary<string, byte[]> Colors { get; protected set; } = new()
        {
            {"Red",[255,0,0] },
            {"Green",[0,255,0] },
            {"Blue",[0,0,255] }
        };

        private Color ToColor(string beadColorName)
        {
            Color color = new();

            byte[]? rgb = [0, 0, 0];

            if (Colors.TryGetValue(beadColorName,out rgb))
            {
                color.A = 255;
                color.R = rgb[0];
                color.G = rgb[1];
                color.B = rgb[2];
            }

            return color;
        }
    }
}
