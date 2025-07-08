using System.ComponentModel;
using System.Windows.Controls;

namespace BeadsAI.UserControls
{
    public partial class NetworkRunControl : UserControl , INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public NetworkRunControl()
        {
            InitializeComponent();
            DataContext = this;

            MessageBus.StrBraceletChanged += StrBraceletUpdateHandler;
            MessageBus.InputChanged += InputUpdateHandler;
        }

        public string[] StrBracelet { get; protected set; } = Array.Empty<string>();

        private string inputformated = $"Input: ";

        public string InputFormated
        {
            get { return inputformated; }
            set
            { 
                inputformated = value;
                OnPropertyChanged(nameof(InputFormated));
            }
        }

        private float[] output = new float[4];

        public float[] Output
        {
            get { return output; }
            set 
            { 
                output = value;
                MessageBus.UpdateOutput(value);
            }
        }

        private void StrBraceletUpdateHandler(string[] strbracelet)
        {
            StrBracelet = strbracelet;
        }

        private void InputUpdateHandler(string input)
        {
            InputFormated = $"Input: {input}";
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Run_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Output = [25.1f, 33.333333333333f, 95, 66.6f];
        }
    }
}
