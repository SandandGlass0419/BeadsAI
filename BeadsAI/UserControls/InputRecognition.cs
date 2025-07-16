using BeadsAI.Core;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Windows;
using Newtonsoft.Json;

namespace BeadsAI.UserControls
{
    public class InputRecognition
    {
        public static readonly (string Dir,string Path) Core = (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Core\InputRecognition\") , "ModelCore.py");
        public readonly string ModelPath = string.Empty;
        public const string tmpfilepath = @"C:\BeadsFolder\tmpfile.jpeg";

        public InputRecognition(string ModelPath)
        {
            this.ModelPath = ModelPath;
        }

        private Process CreateProcess(string ImagePath)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"{Core.Dir + Core.Path}\" \"{ImagePath}\" \"{ModelPath}\"",
                    WorkingDirectory = Core.Dir,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
        }

        public async Task<int> Run(string ImagePath)
        {
            Process process = CreateProcess(ImagePath);

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();



            process.WaitForExit();

            if (process.ExitCode != 0 || output.StartsWith("Error"))
            { ExceptionThrower.Throw($"Python ModelCore.py throwed error: {error}, ({process.ExitCode})"); }

            return Convert.ToInt32(output);
        }

        public static string SaveToFile(Bitmap bitmap,string ImagePath)
        {
            bitmap.Save(ImagePath,ImageFormat.Jpeg);

            return ImagePath;
        }

        public static string SaveToFile(Bitmap bitmap)
        {
            bitmap.Save(tmpfilepath,ImageFormat.Jpeg);

            return tmpfilepath;
        }
    }

    public class ModelClient
    {
        private NamedPipeClientStream pipeClient;
        private StreamWriter writer = new(new MemoryStream());
        private StreamReader reader = new(new MemoryStream());

        public async Task Connect()
        {
            pipeClient = new(".", "ModelCorePipe", PipeDirection.InOut);
            await pipeClient.ConnectAsync();

            writer = new(pipeClient) { AutoFlush = true };
            reader = new(pipeClient);
        }

        public async Task<int> CommandPredict(string ImagePath)
        {
            var request = new
            {
                command = "predict",
                imagepath = ImagePath
            };

            await writer.WriteLineAsync(JsonConvert.SerializeObject(request)); // send
            string response = await reader.ReadLineAsync(); // get

            var result = JsonConvert.DeserializeObject<dynamic>(response);
            return JsonConvert.DeserializeObject<double[]>(result.prediction.ToString());
        }

        public async Task ExitCommand()
        {
            var request = new
            {
                command = "exit",
            };

            await writer.WriteLineAsync(JsonConvert.SerializeObject(request)); // send
            string response = await reader.ReadLineAsync(); // get
        }
    }
}
