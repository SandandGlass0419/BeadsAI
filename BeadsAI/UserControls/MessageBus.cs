namespace BeadsAI.UserControls
{
    public static class MessageBus
    {
        public static Action<string[]>? StrBraceletChanged;
        public static void UpdateStrBracelet(string[] strbracelet) => StrBraceletChanged?.Invoke(strbracelet);

        public static Action<string>? StrInputChanged;
        public static void UpdateStrInput(string strinput) => StrInputChanged?.Invoke(strinput);

        public static Action<float[]>? OutputChanged;
        public static void UpdateOutput(float[] output) => OutputChanged?.Invoke(output);
    }
}
