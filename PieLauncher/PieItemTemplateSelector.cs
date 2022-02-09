using System.Windows;
using System.Windows.Controls;

namespace PieLauncher
{
    public class PieItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? ShortcutDataTemplate { get; set; }
        public DataTemplate? SeparatorDataTemplate { get; set; }
        public DataTemplate? FolderDataTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                ShortcutViewModel => ShortcutDataTemplate,
                SeparatorViewModel => SeparatorDataTemplate,
                FolderViewModel => FolderDataTemplate,
                _ => null
            };
        }
    }
}
