using BeadsAI.Core;
using BeadsAI.Core.NeuralNetwork;
using TorchSharp;

namespace BeadsAI
{
    public class BraceletNetwork : NetworkCore
    {
        public enum WeightLen
        {
            Input = 16,
            Hidden = 8,
            Output = 8
        }

        public enum BiasLen
        {
            Input = 8,
            Hidden = 8,
            Output = 2
        }

        public const long max_model_size = 4;


        protected override torch.Tensor SetBias(WeightColor[] Layer)
        {
            return //ExampleWeights.Increasing(Layer.Length, -0.2f, 1f);
            torch.zeros(Layer.Length);
        }

        protected override bool LayerMetRequirments(WeightColor[] Layer)
        {
            long model_size = Model.Count;

            if (model_size >= max_model_size) // Check if Model is full

            { throw new ArgumentException($"{nameof(Model)}: Maximum size reached. Check {nameof(max_model_size)}."); }


            // Check if Model's last element && Layer has spesified biases
            if (model_size == max_model_size - 1 && Layer.Length != (int) BiasLen.Output)

            { throw new ArgumentException($"{nameof(Layer)} didn't meet requirments for the last element of {nameof(Model)}."); }

            if (model_size != max_model_size - 1 && Layer.Length != (int) BiasLen.Hidden)

            { throw new ArgumentException($"Size of {nameof(Layer)} didn't meet requirments."); }


            // Check if Model's first element && each weights has specified length
            if (model_size == 0 && Layer.First().Weights.Length != (int) WeightLen.Input)

            { throw new ArgumentException($"{nameof(Layer)} didn't meet requirments for the first element of {nameof(Model)}."); }

            if (model_size != 0 && Layer.First().Weights.Length != (int) BiasLen.Hidden)

            { throw new ArgumentException($"{nameof(Layer)} didn't have specified Weight size for each element."); }


            return true;
        }

        protected override bool InputMetRequirments(torch.Tensor Input) // Input for run
        {
            if (Input.size(1) != (long) WeightLen.Input)
            { throw new ArgumentException($"{nameof(Input)} didn't have specifed size."); }

            return true;
        }

        public override torch.Tensor ToTensor(float[] input)
        {
            return torch.tensor(input).reshape(1, (int) WeightLen.Input);
        }
    }

    public class InputColor : ColorMap
    {
        protected override bool IsMetRequirements(WeightColor Color)
        {
            if (Colors.ContainsKey(Color.ColorName)) // checks if color to add is already in dictionary
            { throw new ArgumentException($"{nameof(WeightColor)} Already exists in Dictionary."); }

            if (Color.Weights.Length != (int) BraceletNetwork.WeightLen.Input) // checks new color's weight length is as specified
            { throw new ArgumentException($"{nameof(WeightColor.Weights)} Doesn't have specified length."); }

            return true;
        }
    }

    public class HiddenColor : ColorMap
    {
        protected override bool IsMetRequirements(WeightColor Color)
        {
            if (Colors.ContainsKey(Color.ColorName))
            { throw new ArgumentException($"{nameof(WeightColor)} Already exists in Dictionary."); }
            
            if (Color.Weights.Length != (int) BraceletNetwork.WeightLen.Hidden)
            { throw new ArgumentException($"{nameof(WeightColor.Weights)} Doesn't have specified length."); }

            return true;
        }
    }

    public class OutputColor : ColorMap
    {
        protected override bool IsMetRequirements(WeightColor Color)
        {
            if (Colors.ContainsKey(Color.ColorName))
            { throw new ArgumentException($"{nameof(WeightColor)} Already exists in Dictionary."); }

            if (Color.Weights.Length != (int) BraceletNetwork.WeightLen.Output)
            { throw new ArgumentException($"{nameof(WeightColor.Weights)} Doesn't have specified length."); }

            return true;
        }
    }
}
