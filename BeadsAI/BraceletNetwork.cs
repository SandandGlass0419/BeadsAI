using BeadsAI.Core.NeuralNetwork;

namespace BeadsAI
{
    public class BraceletNetwork : NetworkConfig
    {
        public BraceletNetwork()
        {
            Bias = (8, 8, 2); // (input,hidden,output)
            MaxModelSize = 5; // 8+8+8+8+2
            InputSize = 25; // must be same as InputColor.WeightLen

            OutputNeurons = ["Out1,Out2"];
        }

        public string[] CurrentBracelet {  get; protected set; } = Array.Empty<string>();

        public void AddLayers(string[] Bracelet)
        {
            CurrentBracelet = Bracelet;

            ClearModel();


            var chunked = Bracelet.Chunk(MaxModelSize - 1).ToArray();

            AddInputLayer(InputColor.ToWeightColors(chunked[0]));

            for (int i = 1; i < MaxModelSize - 1; i++)
            {
                AddHiddenLayer(HiddenColor.ToWeightColors(chunked[i]));
            }

            AddOutputLayer(OutputColor.ToWeightColors(OutputNeurons));
        }

        public float[] RunModel(float[] input)
        {
            return ToArray(Run(ToTensor(input)));
        }

        // Model testing part

        public Dictionary<float[], int> TestInputs { get; protected set; } = new() // input, expected answer
        {
            { [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0] , 0 } // starts from 0
        };

        public void TestModel()
        {
            int score = 0;

            foreach (var input in TestInputs.Keys)
            {
                float[] result = RunModel(input);
                float max_element = result.Max();

                if (Array.IndexOf(result, max_element) == TestInputs[input])
                { score++; }
            }

            WriteResults(score);
        }

        private void WriteResults(int score)
        {
            string path = "C:\\BraceletLogs\\Logs.txt"; // must create file path before start
            
            System.IO.File.AppendAllText(path, CurrentBracelet.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText(path, score.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText(path,Environment.NewLine);
        }
    }

    public class InputColor : WeightColorConfig
    {
        public InputColor()
        {
            WeightLen = 25; // must be same as NetworkConfig.InputSize

            Weights = new()
            {
                {"Red", ExampleWeights.OneValue(WeightLen,0.5f) }
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
                {"Red", ExampleWeights.OneValue(WeightLen,0.5f) }
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
                {"Out", ExampleWeights.OneValue(WeightLen,0.5f) }
            };

            ColorMetRequirements();
        }
    }
}
