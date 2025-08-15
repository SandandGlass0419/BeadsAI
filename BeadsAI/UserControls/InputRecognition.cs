using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace BeadsAI.UserControls
{
    public class InputRecognition
    {
        public static readonly (string Dir, string Path) Core = (@"C:\Projects\C#\BeadsAI\BeadsAI\Core\InputRecognition\", "IOtest.py");

        Process python = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{Core.Dir + Core.Path}\"",
                WorkingDirectory = Core.Dir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            },

            
        };

        public async Task<string?> Run()
        {
            python.Start();

            using var reader = python.StandardOutput;
            using var writer = python.StandardInput;
            writer.AutoFlush = true;

            writer.WriteLine("hehehehaw");

            string? line = await reader.ReadLineAsync();

            //writer.WriteLine("Aba");

            return line;
        }
    }
}
