using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ResonanceExtractor.Helpers;
using System.IO;

namespace ResonanceExtractor;

public partial class RootWindow : Window
{
    public RootWindow()
    {
        InitializeComponent();
    }

    private void Window_Closing(object? sender, Avalonia.Controls.WindowClosingEventArgs e)
    {
        if (Directory.Exists(Constants.WemFolderLocation()))
        {
            Directory.Delete(Constants.WemFolderLocation(), true);
        }
    }
}