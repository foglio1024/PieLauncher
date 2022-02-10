using Dragablz;
using Nostrum.WPF.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PieLauncher
{
    public partial class ConfigWindow : Window
    {
        readonly List<object> _order = new();

        public ConfigWindow()
        {
            InitializeComponent();
            AddHandler(DragablzItem.DragCompleted, new DragablzDragCompletedEventHandler(ItemDragCompleted), true);
        }

        void ItemDragCompleted(object sender, DragablzDragCompletedEventArgs e)
        {
            var ic = e.DragablzItem.FindVisualParent<DragablzItemsControl>();
            if (ic == null) return;
            var src = (ObservableCollection<IPieItem>)ic.ItemsSource;
            var done = false;
            if (!_order.Any(x => src.Contains(x))) return;
            while (!done)
            {
                for (int i = 0; i < src.Count; i++)
                {
                    var newIndex = _order.IndexOf(src[i]);
                    var oldIndex = i;
                    if (newIndex != oldIndex)
                    {
                        src.Move(oldIndex, newIndex);
                        break;
                    }
                    if (i == src.Count - 1)
                    {
                        done = true;
                    }
                }
            }
        }

        void OnOrderChanged(object sender, OrderChangedEventArgs e)
        {
            _order.Clear();
            _order.AddRange(e.NewOrder);
        }
    }
}
