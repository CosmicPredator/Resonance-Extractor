using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using ResonanceExtractor.Helpers;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Threading.Tasks;

namespace ResonanceExtractor;

public partial class ExistingPatchPage : UserControl
{
    private string PCKFilePath { get; set; } = string.Empty;
    private string OutputFolderPath { get; set; } = string.Empty;

    public ExistingPatchPage()
    {
        InitializeComponent();
    }

    private async void ChooseFileButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var currentTopLevel = TopLevel.GetTopLevel(this);
        var files = await currentTopLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open PCK File",
            AllowMultiple = false,
            FileTypeFilter = [
                new FilePickerFileType("Genshin Impact PCK Game File")
                {
                    Patterns = ["*.pck"]
                }
            ]
        });

        if (files.Count == 1)
        {
            PCKFilePath = files[0].Path.LocalPath;
            PCKFilePathTB.Text = files[0].Name;
        }
    }

    private async void OutputFolderButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var currentTopLevel = TopLevel.GetTopLevel(this);
        var folder = await currentTopLevel!.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                AllowMultiple = false,
                Title = "Choose Output Folder"
            });

        if (folder.Count == 1)
        {
            OutputFolderPath = folder[0].Path.AbsolutePath;
            OutputFolderTB.Text = folder[0].Path.AbsolutePath;
        }
    }

    private async void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await ProcessPCK();
    }

    private void Button_Click_2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SwitchProgressViews(true);
    }

    private void SwitchProgressViews(bool switchToProgressView = false)
    {
        PaimonImage.IsVisible = !switchToProgressView;
        ProgressView.IsVisible = switchToProgressView;
    }

    private async Task ProcessWem()
    {
        PckWemConvert convert = new(PCKFilePath);
        await convert.ConvertAsync();
    }

    private async Task ProcessWav()
    {
        DirectoryInfo dirInfo = new DirectoryInfo(Constants.WemFolderLocation());
        IndividualTaskProgressBar.Value = 0;
        IndividualTaskProgressBar.Maximum = dirInfo.GetFiles().Length;

        if (!Directory.Exists($"{OutputFolderPath}/WAV"))
        {
            Directory.CreateDirectory($"{OutputFolderPath}/WAV");
        }

        foreach (var file in dirInfo.GetFiles())
        {
            var WavWemConvert = new WemWavConvert(
                $"{OutputFolderPath}/WAV/{file.Name.Replace(".wem", ".wav")}", file.FullName);
            await WavWemConvert.ConvertAsync();
            IndividualTaskProgressBar.Value++;
        }
    }

    private async Task PostProcessFormats(FileType outputType)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo($"{OutputFolderPath}\\WAV");
        IndividualTaskProgressBar.Value = 0;
        IndividualTaskProgressBar.Maximum = directoryInfo.GetFiles().Length;

        var folderName = outputType switch
        {
            FileType.MP3 => "MP3",
            FileType.FLAC => "FLAC",
            FileType.OGG => "OGG",
            _ => "FLAC"
        };

        var extensionName = outputType switch
        {
            FileType.MP3 => ".mp3",
            FileType.FLAC => ".flac",
            FileType.OGG => ".ogg",
            _ => ".flac"
        };

        if (!Directory.Exists($"{OutputFolderPath}\\{folderName}"))
        {
            Directory.CreateDirectory($"{OutputFolderPath}\\{folderName}");
        }

        foreach (var file in directoryInfo.GetFiles())
        {
            var wavOtherConverter = new WavOtherFormatConvert(file.FullName,
                $"{OutputFolderPath}\\{folderName}\\{file.Name.Replace(".wav", extensionName)}",
                outputType);
            await wavOtherConverter.ConvertAsync();
            IndividualTaskProgressBar.Value++;
        }
    }

    private async Task ProcessPCK()
    {
        SwitchProgressViews(true);

        await ProcessWem();
        Debug.WriteLine("Processed Wem...!");

        IndividualProgressStatusText.Text = "Converting WEM => WAV...";
        await ProcessWav();
        Debug.WriteLine("Processed Wav...!");

        IndividualProgressStatusText.Text = "Converting WAV => FLAC...";
        await PostProcessFormats(FileType.FLAC);
        Debug.WriteLine("Processed Flac...!");

        IndividualProgressStatusText.Text = "Converting WAV => MP3...";
        await PostProcessFormats(FileType.MP3);
        Debug.WriteLine("Processed Mp3...!");

        IndividualProgressStatusText.Text = "Converting WAV => OGG...";
        await PostProcessFormats(FileType.OGG);
        Debug.WriteLine("Processed Ogg...!");

        SwitchProgressViews(false);
    }
}