using Nostrum.WPF.Extensions;
using System.Windows;

namespace PieLauncher
{
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
        }

        void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.TryDragMove();
        }

        void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
