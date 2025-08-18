using BeadsAI.Core;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BeadsAI.UserControls
{
    public class ModelService
    {
        public static readonly (string Dir, string Path) Process = (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Core\InputRecognition\"), @"ModelService.py");
        public static readonly string ModelPath = @"C:\BeadsFolder\Model\Model.h5";
        public static readonly string TmpImgPath = @"C:\BeadsFolder\tmpfile.jpg";

        private Process service = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{Process.Dir + Process.Path}\" \"{ModelPath}\" \"{TmpImgPath}\"",
                WorkingDirectory = Process.Dir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            }
        };

        private StreamWriter writer;
        private StreamReader reader;

        public ModelService()
        {
            service.Start();

            writer = service.StandardInput;
            reader = service.StandardOutput;
        }

        public void Close()
        {
            if (service.HasExited)
            { return; }

            writer.Close();
            reader.Close();
            service.Close();

            writer.Dispose();
            reader.Dispose();
            service.Dispose();
        }

        public void Send(string command)
        {
            if (service.HasExited)
            { ExceptionThrower.Throw($"'{nameof(service)}' has already been disposed."); }

            writer.WriteLine(command);
            writer.Flush();
        }

        public async Task<string> SendGetAsync(string command)
        {
            if (service.HasExited)
            { ExceptionThrower.Throw($"'{nameof(service)}' has already been disposed."); }

            Send(command);

            string? responce = await reader.ReadLineAsync();

            return responce ?? string.Empty;
        }
    }

    public class InputRecognition
    {
        private enum Commands
        {
            Load,
            Classify
        };

        private enum Responce
        {
            Success
        }

        private ModelService Service = new();

        public async Task Initialize()
        {
            string responce = await Service.SendGetAsync(Commands.Load.ToString()); 

            CheckResponceInit(responce);
        }

        private void CheckResponceInit(string responce)
        {
            if (responce != Responce.Success.ToString())
            { ExceptionThrower.Throw($"python: {responce}"); }
        }

        public async Task<int> RunModel()
        {
            string responce = await Service.SendGetAsync(Commands.Classify.ToString());

            return CheckResponceRunModel(responce);
        }

        private int CheckResponceRunModel(string responce)
        {
            if (!int.TryParse(responce, out int result))
            { ExceptionThrower.Throw($"python: {responce}"); }

            if (result < 0 || result >= InputSelectControl.StrInputs.Count)
            { ExceptionThrower.Throw("Model responce out of bounds."); }

            return result;
        }

        public static string SaveToFile(Bitmap bitmap, string ImagePath)
        {
            bitmap.Save(ImagePath, ImageFormat.Jpeg);

            return ImagePath;
        }

        public static string SaveToFile(Bitmap bitmap)
        {
            bitmap.Save(ModelService.TmpImgPath, ImageFormat.Jpeg);

            return ModelService.TmpImgPath;
        }

        public void TerminateModel()
        {
            Service.Close();
        }
    }
}
