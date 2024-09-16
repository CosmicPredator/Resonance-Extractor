using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResonanceExtractor.Helpers
{
    public class ProcessExecutor
    {
        private readonly string ExecutableName;
        private readonly string Arguments;
        public ProcessExecutor(string executableName, string arguments)
        {
            ExecutableName = executableName;
            Arguments = arguments;
        }
        public bool Execute()
        {
            using (Process process = new Process())
            {
                try
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        FileName = ExecutableName,
                        Arguments = Arguments,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    };
                    process.Start();
                    process.WaitForExit();
                    Debug.WriteLine(process.StandardOutput.ReadToEnd().ToString());
                    Debug.WriteLine(process.StandardError.ReadToEnd().ToString());
                    return true;
                } catch (Exception ex)
                {
                    Debug.WriteLine($"Error executing {ex}");
                    return false;
                }
            }
        }
    }
}
