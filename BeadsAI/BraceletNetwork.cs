using BeadsAI.Core.NeuralNetwork;
using System.IO;

namespace BeadsAI
{
    public class BraceletNetwork : NetworkConfig
    {
        public BraceletNetwork()
        {
            Neurons = (8, 2); // (General,Output), General = Input, Hidden
            MaxModelSize = 5; // 8+8+8+8+2
            InputSize = 25; // must be same as InputColor.WeightLen

            OutputNeurons = ["Out1","Out2"]; // Length must be same as Neurons.Output
        }

        public void AddLayers(string[] Bracelet)
        {
            BraceletMetRequirments(Bracelet);

            ClearModel();

            InputColor inputcolor = new(); HiddenColor hiddencolor = new(); OutputColor outputcolor = new();
            var layers = Bracelet.Chunk(Neurons.General).ToArray();

            AddInputLayer(inputcolor.ToWeightColors(layers[0]));

            AddAllHiddenLayers(hiddencolor,layers);

            AddOutputLayer(outputcolor.ToWeightColors(OutputNeurons));
        }

        private void AddAllHiddenLayers(HiddenColor hiddencolor, string[][] layers)
        {
            layers = layers.Skip(1).ToArray();

            foreach (string[] layer in layers)
            {
                AddHiddenLayer(hiddencolor.ToWeightColors(layer));
            }
        }

        public float[] RunModel(float[] input)
        {
            return ToArray(Run(ToTensor(input)));
        }

        public void Evaluate(string[] Bracelet)
        {
            Evaluator eval = new();

            foreach (var inputkpv in eval.TestInputs)
            {
                eval.Score(RunModel(inputkpv.Key), inputkpv.Value);
            }

            eval.WriteResults(Bracelet);
        }
    }

    public class InputColor : WeightColorConfig
    {
        public InputColor()
        {
            WeightLen = 25; // must be same as NetworkConfig.InputSize

            Weights = new()
            {
                {"Red", ExampleWeights.OneValue(WeightLen,0.5f) },
                {"Blue", ExampleWeights.OneValue(WeightLen,0.7f) },
                {"Green", ExampleWeights.OneValue(WeightLen,-0.3f) }
            };

            ColorMetRequirements();
        }
    }

    public class HiddenColor : WeightColorConfig
    {
        public HiddenColor()
        {
            WeightLen = 8;

            Weights = new()
            {
                {"Red", ExampleWeights.OneValue(WeightLen,0.5f) },
                {"Blue", ExampleWeights.OneValue(WeightLen,0.7f) },
                {"Green", ExampleWeights.OneValue(WeightLen,-0.3f) }
            };

            ColorMetRequirements();
        }
    }

    public class OutputColor : WeightColorConfig
    {
        public OutputColor()
        {
            WeightLen = 8;

            Weights = new()
            {
                {"Out1", ExampleWeights.OneValue(WeightLen,0.5f) },
                {"Out2", ExampleWeights.OneValue(WeightLen,0.2f) }
            };

            ColorMetRequirements();
        }
    }

    public class Evaluator
    {
        public Dictionary<float[], int> TestInputs { get; protected set; } = new() // input, expected answer
        {
            { [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0] , 0 } // starts from 0
        };

        public int score = 0;

        private (string folder, string file) path = ("C:\\BeadsLog\\", "Log.txt");

        public void Score(float[] result,int expected)
        {
            if (Array.IndexOf(result,result.Max()) == expected)
            { score++; }
        }

        public void WriteResults(string[] Bracelet)
        {
            AddDir(path);

            File.AppendAllText(path.folder + path.file, string.Join(',',Bracelet) + Environment.NewLine);
            File.AppendAllText(path.folder + path.file, score.ToString() + Environment.NewLine);
            File.AppendAllText(path.folder + path.file, Environment.NewLine);
        }

        private void AddDir((string folder,string file) path)
        {
            if (!File.Exists(path.folder + path.file))
            {
                Directory.CreateDirectory(path.folder);
                File.Create(path.folder + path.file);
            }
        }
    }
}
