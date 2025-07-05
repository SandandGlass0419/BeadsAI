using System.Windows;

namespace BeadsAI.UserControl
{
    public partial class NetworkRunControl
    {
        public NetworkRunControl()
        {
            InitializeComponent();
            DataContext = this;

            MessageBus.strBraceletChanged += strBraceletUpdateHandler;
        }

        public string Input { get; protected set; } = String.Empty;
        public string[] strBracelet { get; protected set; } = Array.Empty<string>();

        private void strBraceletUpdateHandler(string[] strBracelet)
        {
            this.strBracelet = strBracelet;
        }

    }
}
