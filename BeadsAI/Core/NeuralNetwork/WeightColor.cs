namespace BeadsAI.Core
{
    public sealed class WeightColor
    {
        public string ColorName { get; private set; }
        public float[] Weights { get; private set; }

        public WeightColor(string ColorName, float[] Weights)
        {
            this.ColorName = ColorName;
            this.Weights = Weights;
        }

        public static WeightColor Create(string ColorName, float[] Weights)
        {
            return new WeightColor(ColorName, Weights);
        }
    }

    // mother class for storing color definitions
    public abstract class ColorMap
    {
        public Dictionary<string, WeightColor> Colors { get; private set; } = new();

        public void Add(WeightColor Color)
        {
            IsMetRequirements(Color);

            Colors.Add(Color.ColorName,Color);
        }

        public WeightColor[] ToColors(string[] string_colors)
        {
            WeightColor[] color_array = Array.Empty<WeightColor>();

            foreach (string str in string_colors)
            {
                if (Colors.TryGetValue(str,out WeightColor? element))
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

        protected abstract bool IsMetRequirements(WeightColor Color); // check if new Color is valid
    }
}
