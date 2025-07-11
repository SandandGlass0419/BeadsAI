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

            Weights = new()
            {
                {"Red", ExampleWeights.OneValue(WeightLen,-0.4f) },
                {"Blue", ExampleWeights.OneValue(WeightLen,-0.3f) },
                {"Green", ExampleWeights.OneValue(WeightLen,-0.2f) },
                {"LightGreen", ExampleWeights.OneValue(WeightLen,-0.1f) }, // gr2
                {"SkyBlue", ExampleWeights.OneValue(WeightLen,0.1f) }, // ls
                {"LightYellow", ExampleWeights.OneValue(WeightLen,0.2f) }, // ly
                {"LightPink", ExampleWeights.OneValue(WeightLen,0.3f) }, // p1
                {"Purple", ExampleWeights.OneValue(WeightLen,0.4f) } // pp
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
                {"Red", ExampleWeights.OneValue(WeightLen,-0.4f) },
                {"Blue", ExampleWeights.OneValue(WeightLen,-0.3f) },
                {"Green", ExampleWeights.OneValue(WeightLen,-0.2f) },
                {"LightGreen", ExampleWeights.OneValue(WeightLen,-0.1f) }, // gr2
                {"SkyBlue", ExampleWeights.OneValue(WeightLen,0.1f) }, // ls
                {"LightYellow", ExampleWeights.OneValue(WeightLen,0.2f) }, // ly
                {"LightPink", ExampleWeights.OneValue(WeightLen,0.3f) }, // p1
                {"Purple", ExampleWeights.OneValue(WeightLen,0.4f) } // pp
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
                {"Out2", ExampleWeights.OneValue(WeightLen,0.2f) },
                {"Out3", ExampleWeights.OneValue(WeightLen,-0.4f) },
                {"Out4", ExampleWeights.OneValue(WeightLen,-0.1f) }
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

        private static (string folder, string file) path = ("C:\\BeadsLog\\", "Log.txt");
        
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