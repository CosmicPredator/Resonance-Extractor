using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ResonanceExtractor;

public partial class MainPage : UserControl
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void TabStrip_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
    }
}