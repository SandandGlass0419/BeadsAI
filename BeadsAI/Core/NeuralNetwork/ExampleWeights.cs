namespace BeadsAI.Core.NeuralNetwork
{
    public static class ExampleWeights
    {
        public static float Amplifyer { get; set; } = 0.7f;

        // value value value value ...
        public static float[] OneValue(int length,float value)
        {
            return Enumerable.Repeat(value * Amplifyer, length).ToArray();
        }

        // value1 value2 value1 value2 ...
        public static float[] Striped(int length, float value1, float value2)
        {
            float[] vector = new float[length];

            value1 *= Amplifyer;
            value2 *= Amplifyer;

            for (int i = 0; i < length / 2; i++)
            {
                vector[i] = value1;
                vector[i] = value2;
            }

            if (length % 2 == 1)
            { vector[length - 1] = value1; }

            return vector;
        }

        // start start+difference start+2*difference ...
        public static float[] Increasing(int length, float start, float difference)
        {
            start *= Amplifyer;

            var range = Enumerable.Range(0, length);

            return range.Select(index => start + index * difference).ToArray();
        }
    }
}
