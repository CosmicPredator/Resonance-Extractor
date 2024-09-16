using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResonanceExtractor.Helpers
{
    public class PckWemConvert : IFormatConverter
    {
        private readonly string PCKPath;
        public PckWemConvert(string pckPath)
        {
            PCKPath = pckPath;
        }

        public async Task<bool> ConvertAsync()
        {
            var formatString = string.Format(
                Constants.QuickBMSArgs, Constants.QuickBMSScript, PCKPath, Constants.WemFolderLocation());

            ProcessExecutor executor = new(
                Constants.QuickBMSEXE, formatString);

            return await Task.Run(executor.Execute);
        }
    }
}
