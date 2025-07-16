using BeadsAI.Core.NeuralNetwork;
using System.Diagnostics;
using System.IO;

namespace BeadsAI
{
    public class BraceletNetwork : NetworkConfig
    {
        public BraceletNetwork()
        {
            Neurons = (8, 4); // (General,Output), General = Input, Hidden
            MaxModelSize = 5; // 8+8+8+8+4, 32 length bracelet
            InputSize = 25; // must be same as InputColor.WeightLen

            OutputNeurons = ["Out1","Out2","Out3","Out4"]; // Length must be same as Neurons.Output
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

        public void EvaluateModel(string[] Bracelet)
        {
            Evaluator eval = new();

            foreach (var inputkpv in Evaluator.TestInputs)
            {
                eval.AddScore(RunModel(inputkpv.Key), inputkpv.Value);
            }

            eval.WriteResults(Bracelet);
        }
    }

    public class InputColor : WeightColorConfig
    {
        public InputColor()
        {
            WeightLen = 25; // must be same as NetworkConfig.InputSize
            ExampleWeights.Amplifyer = 0.7f;

            Weights = new()
            {
                {"Red", ExampleWeights.Striped(WeightLen,-0.6f,0.4f) },
                {"Blue", ExampleWeights.Increasing(WeightLen,0.3f,-0.0179f) },
                {"Green", ExampleWeights.OneValue(WeightLen,0.35f) },
                {"LightGreen", ExampleWeights.Striped(WeightLen,0.4f,-0.57f) }, // gr2
                {"SkyBlue", ExampleWeights.Increasing(WeightLen,-0.86f,0.07f) }, // ls
                {"LightYellow", ExampleWeights.Striped(WeightLen,0.67f,-0.034f) }, // ly
                {"LightPink", ExampleWeights.Increasing(WeightLen,-0.3f,0.083f) }, // p1
                {"Purple", ExampleWeights.OneValue(WeightLen,-0.79f) } // pp
            };

            ColorMetRequirements();
        }
    }

    public class HiddenColor : WeightColorConfig
    {
        public HiddenColor()
        {
            WeightLen = 8;
            ExampleWeights.Amplifyer = 0.65f;

            Weights = new()
            {
                {"Red", ExampleWeights.Increasing(WeightLen,-0.47f,0.15f) },
                {"Blue", ExampleWeights.OneValue(WeightLen,0.57f) },
                {"Green", ExampleWeights.Striped(WeightLen,-0.2f,0.55f) },
                {"LightGreen", ExampleWeights.Increasing(WeightLen,0.65f,-0.18f) }, // gr2
                {"SkyBlue", ExampleWeights.OneValue(WeightLen,0.83f) }, // ls
                {"LightYellow", ExampleWeights.Increasing(WeightLen,0.34f,-0.04f) }, // ly
                {"LightPink", ExampleWeights.Striped(WeightLen,0.65f,-0.54f) }, // p1
                {"Purple", ExampleWeights.Striped(WeightLen,-0.43f,-0.15f) } // pp
            };

            ColorMetRequirements();
        }
    }

    public class OutputColor : WeightColorConfig
    { 
        public OutputColor()
        {
            WeightLen = 8;
            ExampleWeights.Amplifyer = 0.75f;

            Weights = new()
            {
                {"Out1", ExampleWeights.Increasing(WeightLen,-0.6f,0.15f) },
                {"Out2", ExampleWeights.Striped(WeightLen,0.53f,-0.12f) },
                {"Out3", ExampleWeights.Striped(WeightLen,-0.4f,0.043f) },
                {"Out4", ExampleWeights.Increasing(WeightLen,0.35f,-0.039f) }
            };

            ColorMetRequirements();
        }
    }

    public class Evaluator
    {
        public static Dictionary<float[], int> TestInputs { get; protected set; } = new() // input, expected answer
        {
            {[0,0,0,0,0,0,0,1,0,0,0,1,1,1,0,0,0,1,0,0,0,0,1,0,0], 0}, // value: 0,1,2,3
            {[0,0,1,0,0,0,0,1,0,0,1,1,1,1,1,0,0,1,0,0,0,0,1,0,0], 0},
            {[0,1,0,0,0,1,1,1,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0], 0},
            {[0,0,0,1,0,0,0,1,1,1,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0], 0},
            {[0,0,0,0,0,0,1,0,0,0,1,1,1,0,0,0,1,0,0,0,0,1,0,0,0], 0},
            {[0,0,0,0,0,0,0,0,1,0,0,0,1,1,1,0,0,0,1,0,0,0,0,1,0], 0},
            {[0,0,1,0,0,0,1,1,1,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0], 0},
            {[0,0,0,0,0,0,0,1,0,0,0,1,1,1,0,0,0,1,0,0,0,0,1,0,0], 0},

            {[0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,1,0,0,0,1,1,1,1,1,1], 1},
            {[0,0,0,0,0,1,1,0,1,1,0,0,0,0,0,1,0,0,0,1,1,1,1,1,1], 1},
            {[0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,1,0,0,0,1,0,1,1,1,0], 1},
            {[0,0,0,0,0,1,1,0,1,1,0,0,0,0,0,1,0,0,0,0,0,1,1,1,0], 1},
            {[0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,1,1,1,0,1,0,0,0,1], 1},
            {[0,0,0,0,0,1,1,0,1,1,0,0,0,0,0,1,1,1,1,1,1,0,0,0,1], 1},
            {[0,0,0,0,0,1,1,0,1,1,0,0,0,0,0,0,1,1,1,0,1,0,0,0,1], 1},

            {[0,1,0,1,0,1,0,1,0,1,1,0,0,0,1,0,1,0,1,0,0,0,1,0,0], 2},
            {[1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0], 2},
            {[0,0,1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0], 2},
            {[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,1,0,0,0], 2},
            {[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,1,0], 2},
            {[0,0,0,0,0,0,1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0], 2},
            {[0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,1,0,0,0,0,0,0,0], 2},

            {[0,0,1,0,0,0,1,0,1,0,1,0,0,0,1,1,0,0,0,1,1,1,1,1,1], 3},
            {[0,0,1,0,0,0,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1], 3},
            {[0,0,1,0,0,0,1,1,1,0,1,1,1,1,1,1,1,1,0,1,1,1,1,0,1], 3},
            {[0,0,1,0,0,0,1,1,1,0,1,1,1,1,1,1,1,0,1,1,1,1,0,1,1], 3},
            {[0,0,1,0,0,0,1,1,1,0,1,1,1,1,1,1,0,1,1,1,1,0,1,1,1], 3}
        };

        public float Score { get; protected set; } = 0;
        public float MaxScore { get; protected set; } = 100 * TestInputs.Count;

        private static (string folder, string file) path = ("C:\\BeadsFolder\\", "Log.txt");
        
        public void AddScore(float[] result,int expected)
        {
            Score += result[expected] * 100;
        }

        public void OldScore(float[] result,int expected)
        {
            if (Array.IndexOf(result, result.Max()) == expected)
            { Score++; }
        }

        public void WriteResults(string[] Bracelet)
        {
            AddDir(path);

            File.AppendAllText(path.folder + path.file, $"Bracelet: {string.Join(',',Bracelet)}" + Environment.NewLine);
            File.AppendAllText(path.folder + path.file, $"Score: {Score} / {MaxScore}" + Environment.NewLine);
            File.AppendAllText(path.folder + path.file, Environment.NewLine);
        }

        public static void OpenLogs()
        {
            AddDir(path);

            Process.Start("notepad.exe",path.folder + path.file);
        }

        private static void AddDir((string folder,string file) path)
        {
            if (!File.Exists(path.folder + path.file))
            {
                Directory.CreateDirectory(path.folder);
                File.WriteAllText(path.folder + path.file, "");
            }
        }
    }
}