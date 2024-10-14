using System.Windows;
using Nostrum.WPF.Extensions;

namespace PieLauncher.Windows;

public partial class ConfigWindow
{
    public ConfigWindow()
    {
        InitializeComponent();
    }

    private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        this.TryDragMove();
    }

    private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}