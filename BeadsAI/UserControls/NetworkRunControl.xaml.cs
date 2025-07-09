using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace BeadsAI.UserControls
{
    public partial class NetworkRunControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public NetworkRunControl()
        {
            InitializeComponent();
            DataContext = this;

            MessageBus.StrBraceletChanged += StrBraceletUpdateHandler;
            MessageBus.StrInputChanged += StrInputUpdateHandler;

            StrInputUpdateHandler("Cross"); // fix - maybe use configuration class
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

        public float[] Input { get; protected set; } = Array.Empty<float>();

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

        private bool isrunning = false;
        private bool istesting = false;

        private void StrBraceletUpdateHandler(string[] strbracelet)
        {
            StrBracelet = strbracelet;

            RunEvaluation();
        }

        private void StrInputUpdateHandler(string strinput)
        {
            Input = InputSelectControl.StrInputMap[strinput];
            InputFormated = $"Input: {strinput}";

            UpdateOutput();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Run_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            isrunning = !isrunning;

            btn_Run.Content = isrunning ? "Stop" : "Run";

            UpdateOutput();
        }

        private void UpdateOutput() // by running network
        {
            if (!isrunning)
            { return; }

            BraceletNetwork Network = new();

            Network.AddLayers(StrBracelet);

            Output = Network.RunModel(Input);

            //MessageBox.Show("Updated!");
        }

        private async void RunEvaluation()
        {
            if (isrunning)
            { return; }

            istesting = true;
            btn_ReRun.IsEnabled = false;

            BraceletNetwork Network = new();
            Network.AddLayers(StrBracelet);

            try
            {
                await Task.Run(() => Network.EvaluateModel(StrBracelet));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(),"Exception on Evaluation");
            }
            finally
            {
                btn_ReRun.IsEnabled = true;
            }
        }
    }
}
