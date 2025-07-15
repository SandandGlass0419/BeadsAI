using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Emgu.TF.Lite;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using BeadsAI.Core;
using SixLabors.ImageSharp.Processing;
using System.Runtime.InteropServices;

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

                BitSource = null;
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

                var bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat).ToBitmapSource();

                Dispatcher.Invoke(() =>
                {
                    BitSource = bitmap;
                });
            }

            mat.Dispose();
            Camera.Release();
        }

        private void btn_Camera_Caputre_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InputRecognition inputRecognition = new("Model/input_model.tflite");

            inputRecognition.Run(BitSource);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class InputRecognition
    {
        public const string RootDir = "pack://application:,,,/";
        public static readonly string[] OutPuts = ["Cross", "Face", "Heart", "House"];

        Interpreter Interpreter = new();

        public InputRecognition(string ModelPath)
        {
            Interpreter = new(new FlatBufferModel(RootDir + ModelPath));
            Interpreter.AllocateTensors();
        }

        private int[] GetInputDims()
        {
            int[] dims = Interpreter.GetTensor(0).Dims;

            if (dims.Length != 4)
            { ExceptionThrower.Throw($"{nameof(Interpreter)} didn't have specified tensor length."); }

            if (dims[0] != 1)
            { ExceptionThrower.Throw($"{nameof(Interpreter)} didn't have specified tensor element."); }

            return dims;
        }

        private float[] ToFloats(Image<Rgb24> image)
        {
            float[] floatTensor = Array.Empty<float>();

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image[x, y];

                    floatTensor = [.. floatTensor, pixel.R / 127.5f - 1.0f,
                                                   pixel.G / 127.5f - 1.0f,
                                                   pixel.B / 127.5f - 1.0f];
                }
            }

            return floatTensor;
        }
        
        private Image<Rgb24> ToRgb24(BitmapSource bitsource)
        {
            Image<Rgb24> image;

            using (MemoryStream mstream = new())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitsource));
                encoder.Save(mstream);
                mstream.Seek(0, SeekOrigin.Begin);
                image = SixLabors.ImageSharp.Image.Load<Rgb24>(mstream);
            }

            return image;
        }

        private Image<Rgb24> FormatImageDims(Image<Rgb24> image)
        {
            int[] dims = GetInputDims();

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(dims[1], dims[2]),
                Mode = ResizeMode.Crop
            }));

            return image;
        }
        
        public void Run(BitmapSource bitsource)
        {
            Image<Rgb24> image = FormatImageDims(ToRgb24(bitsource));
            float[] floatTensor = ToFloats(image);

            var inputTensor = Interpreter.GetTensor(0);

            Marshal.Copy(floatTensor, 0, inputTensor.DataPointer, floatTensor.Length);

            var debugres = Interpreter.Invoke();
        }
    }
}