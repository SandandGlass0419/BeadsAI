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

        public string[] StrBracelet { get; protected set; } = BraceletDisplayControl.InitStrBracelet;

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

        private float bestscore;

        public float BestScore
        {
            get { return bestscore; }
            set
            { 
                bestscore = value;
                MessageBus.UpdateBestScore(value);
            }
        }

        private bool isrunning = false; // keep running when true
        private bool istesting = false; // only run when false

        private void StrBraceletUpdateHandler(string[] strbracelet)
        {
            StrBracelet = strbracelet;

            UpdateOutput();
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

        private void btn_Run_Click(object sender, System.Windows.RoutedEventArgs e)
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

        private void btn_ReRun_Click(object sender, RoutedEventArgs e)
        {
            RunEvaluation();
        }

        private async void RunEvaluation()
        {
            if (istesting)
            { return; }

            istesting = true;
            btn_ReRun.IsEnabled = false;

            BraceletNetwork Network = new();
            Network.AddLayers(StrBracelet);

            try
            {
                var score = await Task.Run(() => Network.EvaluateModel(StrBracelet));

                BestScore = score > bestscore ? score : bestscore;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(),"Exception on Evaluation");
            }
            finally
            {
                istesting = false;
                btn_ReRun.IsEnabled = true;
            }
        }
    }
}
