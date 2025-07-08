using System.ComponentModel;
using System.Windows.Controls;

namespace BeadsAI.UserControls
{
    public partial class OutputDisplayControl : UserControl , INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public OutputDisplayControl()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += (sender, eargs) =>
            {
                thisindex = Convert.ToInt32(Tag);
                ThisImgPath = imgpaths[thisindex];
            };

            MessageBus.OutputChanged += UpdateOutputHandler;
        }

        private int thisindex;
        
        private const string rootdir = "pack://application:,,,/Images/";
        private static string[] imgpaths = [rootdir + "Cross.png", rootdir + "Face.png", rootdir + "Heart.png", rootdir + "House.png"];
        
        private string thisimgpath = imgpaths.First();

        public string ThisImgPath
        {
            get { return thisimgpath; }
            set 
            { 
                thisimgpath = value;
                OnPropertyChanged(nameof(ThisImgPath));
            }
        }

        private string formatedoutput = string.Empty;

        public string FormatedOutput
        {
            get { return formatedoutput; }
            set 
            { 
                formatedoutput = value;
                OnPropertyChanged(nameof(FormatedOutput));
            }
        }

        private float output;

        public float Output
        {
            get { return output; }
            set 
            { 
                output = value;
                OnPropertyChanged(nameof(Output));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateOutputHandler(float[] output)
        {
            Output = (float) Math.Round((double) output[thisindex], 1);
            FormatedOutput = $"Class {thisindex + 1}: {Output}%";
        }
    }
}
