using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResonanceExtractor.Helpers
{
    public static class Constants
    {
        public static string QuickBMSEXE = "Libs\\quickbms.exe";
        public static string QuickBMSScript = "Libs\\wavescan.bms";
        public static string QuickBMSArgs = "\"{0}\" \"{1}\" \"{2}\"";

        public static string VgmStreamCliEXE = "Libs\\vgmstream-cli.exe";
        public static string VgmStreamCliArgs = "-o \"{0}\" \"{1}\"";

        public static string FFMpegEXE = "Libs\\ffmpeg.exe";
        public static string FFMpegFLACArgs = "-i \"{0}\" -sample_fmt s16 -ar 44100 \"{1}\"";
        public static string FFMpegMP3Args = "-i \"{0}\" -b:a 320k \"{1}\"";
        public static string FFMpegOGGArgs = "-i \"{0}\" -acodec libvorbis -b:a 320k \"{1}\"";

        public static string WemFolderLocation()
        {
            var localAppDataPath = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData);
            var wemDirectory = Directory.CreateDirectory($@"{localAppDataPath}\WEM");
            return wemDirectory.FullName;
        }
    }
}
