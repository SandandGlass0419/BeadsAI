using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using BeadsAI;

namespace BeadsAI.UserControls
{
    public partial class ToolBarControl : UserControl
    {
        public ToolBarControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string[] strbracelet = Array.Empty<string>();

        public string[] StrBracelet
        {
            get { return strbracelet; }
            set 
            { 
                strbracelet = value;
                MessageBus.UpdateStrBracelet(strbracelet);
            }
        }


        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog FileOpen = new();
            FileOpen.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (FileOpen.ShowDialog() == true)
            {
                StrBracelet = RunRecognition(FileOpen.FileName);
            }
        }

        private void MenuItem_Logs_Click(object sender, RoutedEventArgs e)
        {
            Evaluator.OpenLogs();
        }

        private string[] RunRecognition(string FileName)
        {
            BraceletRecognition Recognizer = new();

            string[] StrBracelet = Recognizer.FindBraceletColors(FileName);

            MessageBox.Show("Finished loading image!", "Task Finished!",MessageBoxButton.OK,MessageBoxImage.Information);

            return StrBracelet;
        }
    }
}
