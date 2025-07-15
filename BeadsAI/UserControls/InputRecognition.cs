using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using BeadsAI.Core;
using System.Drawing;
using System.Drawing.Imaging;

namespace BeadsAI.UserControls
{
    public class InputRecognition
    {
        private readonly string PythonModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Core\InputRecognition\ModelCore.py");
        private readonly string ModelPath = string.Empty;
        private const string tmpfilepath = @"C:\BeadsFolder\tmpfile.jpeg";

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
                    FileName = "py",
                    Arguments = $"\"{PythonModelPath}\" \"{ImagePath}\" \"{ModelPath}\"",
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

        public static string SaveToFile(Bitmap bitmap)
        {
            bitmap.Save(tmpfilepath,ImageFormat.Jpeg);

            return tmpfilepath;
        }
    }
}
