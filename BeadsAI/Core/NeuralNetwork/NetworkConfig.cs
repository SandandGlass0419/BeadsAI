using TorchSharp;

namespace BeadsAI.Core.NeuralNetwork
{
    public class NetworkConfig : NetworkCore
    {
        public (int General, int Output) Neurons { get; protected set; }
        public int MaxModelSize { get; protected set; }
        public int InputSize { get; protected set; } // must be same as InputColor.WeightLen

        public string[] OutputNeurons { get; protected set; } = Array.Empty<string>();

        protected override torch.Tensor SetBias(WeightColor[] Layer)
        {
            return //ExampleWeights.Increasing(Layer.Length, -0.2f, 1f);
            torch.zeros(Layer.Length);
        }

        protected override bool InputLayerMetRequirments(WeightColor[] Layer)
        {
            if (Model.Count != 0) // check if Layer will be the first element of Model
            { throw new Exception($"\"{nameof(Model)}\" did not have required length."); }

            if (Layer.Length != Neurons.General) // check if length of Layer is correct
            { throw new Exception($"\"{nameof(Layer)}\" did not have required length."); }

            // length of Weight is checked on ColorMetRequirements, so didnt implement
            // same for other methods

            return true;
        }

        protected override bool HiddenLayerMetRequirments(WeightColor[] Layer)
        {
            if (!(0 < Model.Count || Model.Count < MaxModelSize - 1)) // check if its not first or last
            { throw new Exception($"\"{nameof(Model)}\" did not have required length."); }

            if (Layer.Length != Neurons.General)
            { throw new Exception($"\"{nameof(Layer)}\" did not have required length."); }

            return true;
        }

        protected override bool OutputLayerMetRequirments(WeightColor[] Layer)
        {
            if (Model.Count != MaxModelSize - 1)
            { throw new Exception($"\"{nameof(Model)}\" did not have required length."); }

            if (Layer.Length != Neurons.Output)
            { throw new Exception($"\"{nameof(Layer)}\" did not have required length."); }

            return true;
        }

        protected override bool InputMetRequirments(torch.Tensor Input) // Input for run
        {
            if (Input.size(1) != InputSize)
            { throw new ArgumentException($"\"{nameof(Input)}\" didn't have required size."); }

            return true;
        }

        protected bool BraceletMetRequirments(string[] Bracelet)
        {
            if (Bracelet.Length != (MaxModelSize - 1) * Neurons.General)
            { throw new ArgumentException($"\"{nameof(Bracelet)}\" did not have required length."); }

            return true;
        }

        public override torch.Tensor ToTensor(float[] input)
        {
            return torch.tensor(input).reshape(1, (int)InputSize);
        }
    }

    public class WeightColorConfig
    {
        public int WeightLen { get; protected set; }
        public Dictionary<string, float[]> Weights { get; protected set; } = new();

        public WeightColor[] ToWeightColors(string[] string_color)
        {
            WeightColor[] color_array = Array.Empty<WeightColor>();

            foreach (string color in string_color)
            {
                if (Weights.TryGetValue(color, out var weight))
                {
                    color_array = [.. color_array, WeightColor.Create(color, weight)];
                }

                else
                { throw new KeyNotFoundException($"\"{color}\" was not found as key for {nameof(string_color)}."); }
            }

            return color_array;
        }

        protected bool ColorMetRequirements()
        {
            foreach (var weight in Weights.Values)
            {
                if (weight.Length != WeightLen)
                { throw new ArgumentException($"\"{nameof(weight)}\" Doesn't have specified length."); }
            }

            return true;
        }
    }
}
