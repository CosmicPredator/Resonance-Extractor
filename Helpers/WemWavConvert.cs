using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ResonanceExtractor.Helpers
{
    public class WemWavConvert : IFormatConverter
    {
        private readonly string InputFilePath;
        private readonly string OutputFilePath;

        public WemWavConvert(string outputFilePath, string inputFilePath)
        {
            OutputFilePath = outputFilePath;
            InputFilePath = inputFilePath;
        }

        public async Task<bool> ConvertAsync()
        {
            var args = string.Format(
                Constants.VgmStreamCliArgs, OutputFilePath, InputFilePath);
            var executor = new ProcessExecutor(Constants.VgmStreamCliEXE, args);
            return await Task.Run(executor.Execute);
        }
    }
}
