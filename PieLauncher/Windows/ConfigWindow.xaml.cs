using Dragablz;
using Nostrum.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace PieLauncher
{
    public partial class ConfigWindow : Window
    {
        List<object> _order = new();
        public ConfigWindow()
        {
            InitializeComponent();
            AddHandler(DragablzItem.DragCompleted, new DragablzDragCompletedEventHandler(ItemDragCompleted), true);
        }

        void ItemDragCompleted(object sender, DragablzDragCompletedEventArgs e)
        {
            var d = (MainViewModel)DataContext;

            var list = d.Apps;
            Debug.WriteLine($"Applying order: {_order.ToCSV()}");
            var done = false;
            while (!done)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var newIndex = _order.IndexOf(list[i]);
                    var oldIndex = i;
                    if (newIndex != oldIndex)
                    {
                        list.Move(oldIndex, newIndex);
                        break;
                    }
                    if (i == list.Count - 1)
                    {
                        done = true;
                    }
                }
            }

            var src = d.Apps;// ItemsList.ItemsSource;
            ItemsList.ItemsSource = null;
            ItemsList.ItemsSource = src;
            Debug.WriteLine($"Apps order: {d.Apps.ToCSV()}");

        }

        void HorizontalPositionMonitor_OrderChanged(object sender, Dragablz.OrderChangedEventArgs e)
        {
            _order.Clear();
            _order.AddRange(e.NewOrder);
        }

    }
}
