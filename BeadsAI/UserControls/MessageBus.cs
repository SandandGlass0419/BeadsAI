namespace BeadsAI.UserControls
{
    public static class MessageBus
    {
        public static Action<string[]>? StrBraceletChanged;
        public static void UpdateStrBracelet(string[] strbracelet) => StrBraceletChanged?.Invoke(strbracelet);

        public static Action<string>? InputChanged;
        public static void UpdateInput(string input) => InputChanged?.Invoke(input);

        public static Action<float[]>? OutputChanged;
        public static void UpdateOutput(float[] output) => OutputChanged?.Invoke(output);
    }
}
