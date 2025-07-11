using System.Windows.Controls;
using System.ComponentModel;
using OpenCvSharp;
using System.Windows.Media.Imaging;
using OpenCvSharp.WpfExtensions;

namespace BeadsAI.UserControls
{
    public partial class InputSelectControl : UserControl , INotifyPropertyChanged
    {
        private VideoCapture Camera = new();
        private bool iscamon = false;
        private Thread camThread = new(() => {});

        public event PropertyChangedEventHandler? PropertyChanged;

        public InputSelectControl()
        {
            InitializeComponent();
            DataContext = this;

            StrInput = "Cross";
        }

        public static Dictionary<string, float[]> StrInputMap { get; } = new()
        {
            { "Cross", [0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0] },
            { "Face", [0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1] },
            { "Heart", [0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0] },
            { "House", [0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1] }
        };

        public static List<string> StrInputs { get; } = StrInputMap.Keys.ToList(); // used on binding

        private string strinput = String.Empty;

        public string StrInput
        {
            get { return strinput; }
            set 
            { 
                strinput = value;
                ImagePath = rootdir + value;
                MessageBus.UpdateStrInput(value);
            }
        }

        private const string rootdir = "pack://application:,,,/Images/";

        private string imagepath = rootdir + StrInputs.First() + ".png";

        public string ImagePath
        {
            get { return imagepath; }
            set 
            {
                imagepath = value + ".png";
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        private BitmapSource? wbitmap;

        public BitmapSource? WBitmap
        {
            get { return wbitmap; }
            set
            { 
                wbitmap = value;
                OnPropertyChanged(nameof(WBitmap));
            }
        }

        private void btn_Camera_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (iscamon) // cam on
            {
                iscamon = false;

                btn_Cam.Content = "Camera (Off)";
                WBitmap = null;
            }

            else // cam off
            {
                iscamon = true;

                camThread = new Thread(CamCapture);
                camThread.Start();

                btn_Cam.Content = "Camera (On)";
            }
        }

        private void CamCapture()
        {
            Camera = new VideoCapture(0); // 0 = default
            Camera.Open(0);

            Mat mat = new();

            while (iscamon && Camera.IsOpened())
            {
                if (!Camera.Read(mat))
                { continue; }

                var bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat);

                Dispatcher.Invoke(() =>
                {
                    WBitmap = bitmap.ToBitmapSource();
                });
            }

            mat.Dispose();
            Camera.Release();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
