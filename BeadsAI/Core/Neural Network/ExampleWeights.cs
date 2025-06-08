using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeadsAI.Core.Neural_Network
{
    public static class ExampleWeights
    {
        // value value value value ...
        public static float[] OneValue(int length,float value)
        {
            return Enumerable.Repeat(value, length).ToArray();
        }

        // value1 value2 value1 value2 ...
        public static float[] Striped(int length, float value1, float value2)
        {
            float[] vector = new float[length];

            for (int i = 0; i < length / 2; i++)
            {
                vector[i] = value1;
                vector[i] = value2;
            }

            if (length % 2 == 1)
            { vector[length] = value1; }

            return vector;
        }

        // start start+difference start+2*difference ...
        public static float[] Increasing(int length, float start, float difference)
        {
            var range = Enumerable.Range(1, length);

            return range.Select(element => element * difference).ToArray();
        }
    }
}
