using BeadsAI;
using BeadsAI.Core;
using BeadsAI.Core.Neural_Network;
using System.Linq;

namespace BraceletTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            InputColor inputColor = new();

            inputColor.Add(WeightColor.Create("Red", ExampleWeights.OneValue((int) BraceletNetwork.WeightLen.Input,0.55f)));
            inputColor.Add(WeightColor.Create("Blue", ExampleWeights.Increasing((int) BraceletNetwork.WeightLen.Input, -0.478f,0.017f)));
            inputColor.Add(WeightColor.Create("Green", ExampleWeights.Increasing((int) BraceletNetwork.WeightLen.Input, -0.99f, 0.05f)));

            HiddenColor hiddenColor = new();

            hiddenColor.Add(WeightColor.Create("Red", ExampleWeights.Increasing((int) BraceletNetwork.WeightLen.Hidden, -0.2f, 0.079f)));
            hiddenColor.Add(WeightColor.Create("Blue", ExampleWeights.Increasing((int)BraceletNetwork.WeightLen.Hidden, 0.638f, -0.12f)));
            hiddenColor.Add(WeightColor.Create("Green", ExampleWeights.OneValue((int) BraceletNetwork.WeightLen.Hidden, 0.35f)));

            OutputColor outputColor = new();

            outputColor.Add(WeightColor.Create("Out", ExampleWeights.Increasing((int) BraceletNetwork.WeightLen.Output, -0.249f,0.104f)));
            outputColor.Add(WeightColor.Create("Out1", ExampleWeights.OneValue((int) BraceletNetwork.WeightLen.Output, 0.57f)));
            //outputColor.Add
            
            BraceletNetwork MyNet = new();

            MyNet.AddLayer(inputColor.ToColors(["Red", "Blue", "Blue", "Green", "Red", "Green", "Blue", "Green"]));
            MyNet.AddLayer(hiddenColor.ToColors(["Red", "Green", "Blue", "Red", "Green", "Red", "Red", "Green"]));
            MyNet.AddLayer(hiddenColor.ToColors(["Blue", "Green", "Blue", "Red", "Green", "Green", "Red", "Green"]));
            MyNet.AddLayer(outputColor.ToColors(["Out", "Out1"]));

            float[] input1 = { 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1 };
            float[] input2 = { 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0 };
            float[] input3 = { 0.5f,0.5f, 0.5f, 0.5f, 0.5f, -0.5f, -0.5f, 0.5f, 0.5f, -0.5f, -0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f };
            float[] input4 = { 0.1273f, 0.3783f, 0.93f, 0.2739f, 0.173748f, 0.17653f, 0.837f, 0.16f, 0.547f, 0.92773f, 1.1523f, 0.368f, 0.135f, 0.1343f, 0.00123f, 0.0983f };

            var result = MyNet.Run(MyNet.ToTensor(input3));

            var outarray = result.data<float>().ToArray();

            Console.WriteLine($"{outarray[0]} , {outarray[1]}");
        }
    }
}
