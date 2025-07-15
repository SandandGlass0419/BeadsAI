using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Markup;

namespace BeadsAI.UserControls
{
    public partial class InputSelectControl : UserControl, INotifyPropertyChanged
    {
        private VideoCapture Camera = new();
        private bool iscamon = false;
        private Thread camThread = new(() => { });

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
                ImagePath = imagedir + value;
                MessageBus.UpdateStrInput(value);
            }
        }

        private const string imagedir = "pack://application:,,,/Images/";

        private string imagepath = imagedir + StrInputs.First() + ".png";

        public string ImagePath
        {
            get { return imagepath; }
            set
            {
                imagepath = value + ".png";
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        private Bitmap? bitmap;
        private BitmapSource? bitsource;

        public BitmapSource? BitSource
        {
            get { return bitsource; }
            set
            {
                bitsource = value;
                OnPropertyChanged(nameof(BitSource));
            }
        }

        private void btn_Camera_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (iscamon) // cam current on
            {
                iscamon = false;

                btn_Cam.Content = "Camera (Off)";
                btn_Cam_Caputre.IsEnabled = false;
            }

            else // cam current off
            {
                iscamon = true;

                camThread = new Thread(CamCapture);
                camThread.Start();

                btn_Cam.Content = "Camera (On)";
                btn_Cam_Caputre.IsEnabled = true;
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
                var bitmapsource = bitmap.ToBitmapSource();

                Dispatcher.Invoke(() =>
                {
                    this.bitmap = bitmap;
                    BitSource = bitmapsource;
                });
            }

            mat.Dispose();
            Camera.Release();
        }

        private const string ModelPath = @"C:\BeadsFolder\Model\keras_model.h5";

        private async void btn_Camera_Caputre_Click(object sender, System.Windows.RoutedEventArgs e) //check
        {
            if (bitmap is null)
            { return; }

            InputRecognition inpRecog = new(ModelPath);

            int result = await inpRecog.Run(InputRecognition.SaveToFile(bitmap));

            StrInput = StrInputs[result];
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}