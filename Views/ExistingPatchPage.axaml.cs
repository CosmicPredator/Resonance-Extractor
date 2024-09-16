using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using System.Diagnostics;

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
            PCKFilePath = files[0].Path.AbsolutePath;
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
}