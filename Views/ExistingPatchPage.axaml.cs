using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ResonanceExtractor.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace ResonanceExtractor;

public partial class ExistingPatchPage : UserControl
{
    private string PCKFilePath { get; set; } = string.Empty;
    private string OutputFolderPath { get; set; } = string.Empty;

    private readonly List<(CheckBox, FileType)> formatsToProcess;

    public ExistingPatchPage()
    {
        InitializeComponent();
        formatsToProcess = new List<(CheckBox checkBox, FileType fileType)>
        {
            (FlacCheckBox, FileType.FLAC),
            (Mp3CheckBox, FileType.MP3),
            (OggCheckBox, FileType.OGG)
        };
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

    private async void StartButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        enableButtons(false);
        await ProcessPCK();
        enableButtons(true);
    }

    private void processOnUIThread(Action callback)
    {
        Dispatcher.UIThread.Invoke(callback);
    }

    private int getNumberOfProcesses()
    {
        int processCount = 0;

        processOnUIThread(() =>
        {
            if (WavCheckBox.IsChecked == true) processCount++;
            if (FlacCheckBox.IsChecked == true) processCount++;
            if (Mp3CheckBox.IsChecked == true) processCount++;
            if (OggCheckBox.IsChecked == true) processCount++;
        });

        return processCount;
    }

    private void enableButtons(bool enable)
    {
        processOnUIThread(() =>
        {
            ChooseFileButton.IsEnabled = enable;
            OutputFolderButton.IsEnabled = enable;
            StartButton.IsEnabled = enable;
            WavCheckBox.IsEnabled = enable;
            FlacCheckBox.IsEnabled = enable;
            Mp3CheckBox.IsEnabled = enable;
            OggCheckBox.IsEnabled = enable;
        });
    }

    private void resetState()
    {
        processOnUIThread(() =>
        {
            PCKFilePath = string.Empty;
            OutputFolderPath = string.Empty;
            PCKFilePathTB.Text = PCKFilePath;
            OutputFolderTB.Text = OutputFolderPath;

            OverallProgress.Value = 0;
            IndividualTaskProgressBar.Value = 0;

            IndividualProgressStatusText.Text = "Individual Process";

            WavCheckBox.IsChecked = true;
            foreach(var (checkBox, _) in formatsToProcess)
            {
                checkBox.IsChecked = false;
            }
        });
    }

    private void SwitchProgressViews(bool switchToProgressView = false)
    {
        processOnUIThread(() =>
        {
            PaimonImage.IsVisible = !switchToProgressView;
            ProgressView.IsVisible = switchToProgressView;
        });
    }

    private async Task ProcessWem()
    {
        PckWemConvert convert = new(PCKFilePath);
        await convert.ConvertAsync();
    }

    private async Task ProcessWav()
    {
        DirectoryInfo dirInfo = new DirectoryInfo(Constants.WemFolderLocation());

        processOnUIThread(() =>
        {
            IndividualTaskProgressBar.Value = 0;
            IndividualTaskProgressBar.Maximum = dirInfo.GetFiles().Length;
        });

        var wavFolderPath = Path.Combine(OutputFolderPath, "WAV");

        if (Directory.Exists(wavFolderPath))
        {
            Directory.Delete(wavFolderPath, true);
        }
        Directory.CreateDirectory(wavFolderPath);

        foreach (var file in dirInfo.GetFiles())
        {
            processOnUIThread(() =>
            {
                IndividualProgressStatusText.Text = $"WEM => WAV: {file.Name}";
            });
            var WavWemConvert = new WemWavConvert(
                Path.Combine(OutputFolderPath, "WAV", file.Name.Replace(".wem", ".wav")), file.FullName);
            await WavWemConvert.ConvertAsync();
            processOnUIThread(() =>
            {
                IndividualTaskProgressBar.Value++;
            });
        }
    }

    private async Task PostProcessFormats(FileType outputType)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(
            Path.Combine(OutputFolderPath, "WAV"));

        processOnUIThread(() =>
        {
            IndividualTaskProgressBar.Value = 0;
            IndividualTaskProgressBar.Maximum = directoryInfo.GetFiles().Length;
        });

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

        var folderPath = Path.Combine(OutputFolderPath, folderName);

        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true);
        }
        Directory.CreateDirectory(folderPath);

        foreach (var file in directoryInfo.GetFiles())
        {
            processOnUIThread(() =>
            {
                IndividualProgressStatusText.Text = $"WAV => {folderName}: {file.Name}";
            });
            var wavOtherConverter = new WavOtherFormatConvert(file.FullName,
                Path.Combine(folderPath, file.Name.Replace(".wav", extensionName)), outputType);
            await wavOtherConverter.ConvertAsync();
            processOnUIThread(() =>
            {
                IndividualTaskProgressBar.Value++;
            });
        }
    }

    private async Task ProcessPCK()
    {
        int processCount = getNumberOfProcesses() + 1;

        SwitchProgressViews(true);

        if (Directory.Exists(OutputFolderPath))
        {
            Directory.Delete(OutputFolderPath, true);
        }

        processOnUIThread(() =>
        {
            OverallProgress.IsIndeterminate = true;
        });
        await ProcessWem();
        Debug.WriteLine("Processed Wem...!");
        processOnUIThread(() =>
        {
            OverallProgress.IsIndeterminate = false;
        });

        processOnUIThread(() =>
        {
            OverallProgress.Maximum = processCount;
            OverallProgress.Value = 0;
        });

        await ProcessWav();
        Debug.WriteLine("Processed Wav...!");
        processOnUIThread(() => OverallProgress.Value++);

        foreach (var (checkBox, fileType) in formatsToProcess)
        {
            var checkBoxIsChecked = Dispatcher.UIThread.Invoke(() =>
                checkBox.IsChecked == true);
            if (checkBoxIsChecked)
            {
                await PostProcessFormats(fileType);
                Debug.WriteLine($"Processed {fileType}...");
                processOnUIThread(() => OverallProgress.Value++);
            }
        }

        Directory.Delete(Constants.WemFolderLocation(), true);

        var wavCheckBoxIsChecked = Dispatcher.UIThread.Invoke(() => 
            WavCheckBox.IsChecked == true);

        if (!wavCheckBoxIsChecked)
        {
            string wavFolderPath = Path.Combine(OutputFolderPath, "WAV");
            Directory.Delete(wavFolderPath, true);
        }

        resetState();

        SwitchProgressViews(false);
    }
}