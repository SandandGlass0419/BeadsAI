using TorchSharp;
using TorchSharp.Modules;
using static TorchSharp.torch;
using static TorchSharp.torch.nn.functional;
using System.Linq;

namespace BeadsAI.Core.NeuralNetwork
{
    public abstract class NetworkCore : nn.Module
    {
        public ModuleList<Linear> Model { get; protected set; } = new();

        public NetworkCore() : base("NetworkCore") { }

        public void AddLayer(WeightColor[] Layer)
        {
            LayerMetRequirments(Layer);

            Model.Add(CreateLinear(Layer));
            RegisterComponents();
        }

        // sets weight, bias using SetWeights, SetBias. Weights use color definition, so is implemented
        // biases are set by user later, so method is set abstract
        private Linear CreateLinear(WeightColor[] Layer)
        {
            long length = Layer.First().Weights.LongLength;
            long amount = Layer.LongLength;
            
            Linear newLinear = nn.Linear(length, amount, true);

            newLinear.weight.detach_();
            newLinear.weight.copy_(torch.nn.Parameter(SetWeights(Layer, length, amount)));

            newLinear.bias.detach_();
            newLinear.bias.copy_(torch.nn.Parameter(SetBias(Layer)));

            return newLinear;
        }

        protected abstract bool LayerMetRequirments(WeightColor[] Layer);

        private Tensor SetWeights(WeightColor[] Layer,long length,long amount)
        {
            float[] Weights = Array.Empty<float>();

            foreach (WeightColor color in Layer)
            {
                Weights = Weights.Concat(color.Weights).ToArray();
            }

            return torch.tensor(Weights).reshape(amount,length);
        }

        protected abstract Tensor SetBias(WeightColor[] Layer);

        public Tensor Run(Tensor Input)
        {
            InputMetRequirments(Input);

            Tensor result = RecursionRun(Input,0);

            var debug = result.data<float>().ToArray();
            Console.WriteLine(string.Join(',',debug));

            return torch.softmax(result, 1);
        }

        private Tensor RecursionRun(Tensor LayerOutput,int index)
        {
            if (index >= Model.Count)
            { return LayerOutput; }

            LayerOutput = relu(Model[index].forward(LayerOutput));

            var debug = LayerOutput.data<float>().ToArray();
            Console.WriteLine(string.Join(',', debug));

            return RecursionRun(LayerOutput, index + 1);
        }

        protected abstract bool InputMetRequirments(Tensor Input);

        public abstract Tensor ToTensor(float[] input);
    }
}