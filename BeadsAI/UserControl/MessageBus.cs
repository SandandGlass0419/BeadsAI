namespace BeadsAI.UserControl
{
    public static class MessageBus
    {
        public static Action<string[]>? strBraceletChanged;
        public static void UpdatestrBracelet(string[] strBracelet) => strBraceletChanged?.Invoke(strBracelet);
    }
}
