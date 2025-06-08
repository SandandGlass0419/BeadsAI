using System.Windows.Media;

namespace BeadsAI.Core.Neural_Network
{
    public sealed class Color
    {
        public string ColorName { get; private set; }
        public float[] Weights { get; private set; }

        public Color(string ColorName, float[] Weights)
        {
            this.ColorName = ColorName;
            this.Weights = Weights;
        }

        public static Color Create(string ColorName, float[] Weights)
        {
            return new Color(ColorName, Weights);
        }

        public override string ToString()
        {
            return ColorName;
        }
    }

    // mother class for storing color definitions
    public abstract class ColorMap
    {
        public Dictionary<string, Color> Colors { get; private set; } = new();

        public void Add(Color Color)
        {
            IsMetRequirements(Color);

            Colors.Add(Color.ColorName,Color);
        }

        public Color[] ToColors(string[] string_colors)
        {
            Color[] color_array = Array.Empty<Color>();

            foreach (string str in string_colors)
            {
                if (Colors.TryGetValue(str,out Color? element))
                {
                    color_array = color_array.Append(element).ToArray();
                }

                else
                {
                    throw new KeyNotFoundException($"{nameof(string_colors)} had non specified Key.");
                }
            }

            return color_array;
        }

        protected abstract bool IsMetRequirements(Color Color); // check if new Color is valid
    }
}
