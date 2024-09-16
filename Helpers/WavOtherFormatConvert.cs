using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResonanceExtractor.Helpers
{
    public class WavOtherFormatConvert : IFormatConverter
    {
        private readonly string InputFilePath;
        private readonly string OutputFilePath;
        private readonly FileType DestinationFileType;

        public WavOtherFormatConvert(string inputFilePath, string outputFilePath, FileType destinationFileType)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
            DestinationFileType = destinationFileType;
        }

        public async Task<bool> ConvertAsync()
        {
            string finalFFMpegArgs = DestinationFileType switch
            {
                FileType.FLAC => string.Format(Constants.FFMpegFLACArgs, InputFilePath, OutputFilePath),
                FileType.MP3 => string.Format(Constants.FFMpegMP3Args, InputFilePath, OutputFilePath),
                FileType.OGG => string.Format(Constants.FFMpegOGGArgs, InputFilePath, OutputFilePath),
                _ => string.Format(Constants.FFMpegFLACArgs, InputFilePath, OutputFilePath)
            };

            var executor = new ProcessExecutor(Constants.FFMpegEXE, finalFFMpegArgs);
            return await Task.Run(executor.Execute);
        }
    }

    public enum FileType
    {
        FLAC,
        MP3,
        OGG
    }
}
