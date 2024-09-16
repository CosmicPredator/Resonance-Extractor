using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResonanceExtractor.Helpers
{
    internal interface IFormatConverter
    {
        Task<bool> ConvertAsync();
    }
}
