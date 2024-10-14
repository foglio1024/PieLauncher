using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Messaging;
using PieLauncher.Core;
using PieLauncher.Core.ViewModels;

namespace PieLauncher.Avalonia.Controls;

[TemplatePart("PART_Container", typeof(Panel), IsRequired = true)]
[TemplatePart("PART_ItemsHost", typeof(ItemsControl), IsRequired = true)]
public class DragDropList : ItemsControl
    , IRecipient<DragDropList.DragOverMessage>
{
    public record DragOverMessage(DragDropList Sender);

    private Panel? _container;
    private ItemsControl? _itemsHost;
    private readonly Rectangle _dragIndicator;
    private Point _startDragPoint;
    private bool _isDragging;

    public bool CanDragDrop
    {
        get => GetValue(CanDragDropProperty);
        set => SetValue(CanDragDropProperty, value);
    }

    public static readonly StyledProperty<bool> CanDragDropProperty =
        AvaloniaProperty.Register<DragDropList, bool>(nameof(CanDragDrop), true);


    public DragDropList()
    {
        AddHandler(Control.PointerPressedEvent, SetupDrag);
        AddHandler(Control.PointerMovedEvent, StartDrag);
        _dragIndicator = CreateSeparator();

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    private async void StartDrag(object? sender, PointerEventArgs e)
    {
        if (!CanDragDrop) return;

        if (_isDragging) return;

        if (_startDragPoint == default) return;

        if (Point.Distance(e.GetPosition(this), _startDragPoint) < 10) return;

        _isDragging = true;
        _dragIndicator.IsVisible = true;

        if (DataContext is not IDragDropHandler context)
        {
            throw new InvalidOperationException("DataContext must be an IDragDropHandler");
        }

        if (e.Source is not Control { DataContext: IDragDropItem item })
        {
            throw new InvalidOperationException("Dragged item must be an IDragDropItem");
        }

        context.StartDrag(item);

        var dragData = new DataObject();
        dragData.Set("item", item);
        dragData.Set("source", _itemsHost?.ItemsSource ?? Array.Empty<IDragDropItem>());

        Debug.WriteLine($"START {item}");
        try
        {
            _ = await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        
    }

    private void SetupDrag(object? sender, PointerPressedEventArgs e)
    {
        if (!CanDragDrop) return;

        if (_isDragging) _isDragging = false;
        _startDragPoint = e.GetPosition(this);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        var parent = this.FindAncestorOfType<DragDropList>();

        if (parent != null)
        {
            parent.CanDragDrop = false;
            CanDragDrop = true;
        }

        if (!CanDragDrop) return;

        if (DataContext is not IDragDropHandler context)
        {
            throw new InvalidOperationException("DataContext must be an IDragDropHandler");
        }

        var data = e.Data.Get("item");
        if (data is not IDragDropItem ddi)
        {
            throw new InvalidOperationException("Dragged item must be an IDragDropItem");
        }

        e.DragEffects = DragDropEffects.Move;

        WeakReferenceMessenger.Default.Send(new DragOverMessage(this));

        if (e.Source is not Control dropTarget)
        {
            throw new InvalidOperationException("idk what happened");
        }

        var dropTargetRoot = (Control?)dropTarget.GetVisualAncestors().FirstOrDefault(x => x.Parent == _itemsHost);

        if (dropTargetRoot == null)
        {
            // we dropped into the ItemsControl
            _dragIndicator.Fill = Brushes.DodgerBlue;
        }
        else
        {
            _dragIndicator.Fill = Brushes.Orange;
            // we dropped over another item in the ItemsControl
            var pointerPos = e.GetPosition(dropTargetRoot);
            var h = dropTargetRoot.Bounds.Height;

            // figure out which vertical half of the control we dropped the item to and decide the index based on that
            var bottom = pointerPos.Y > h / 2f;

            var pos = dropTargetRoot.Bounds.Top;

            if (bottom) pos += dropTargetRoot.Bounds.Height;

            MoveSeparatorTo(pos);
        }



        if (!context.CanDrop(ddi, _itemsHost?.ItemsSource))
        {
            e.DragEffects = DragDropEffects.None;
        }

    }

    private void MoveSeparatorTo(double pos)
    {
        if (_dragIndicator.IsVisible == false) _dragIndicator.IsVisible = true;
        var tx = (TranslateTransform)_dragIndicator.RenderTransform!;
        tx.Y = pos;
    }

    private void Drop(object? sender, DragEventArgs e)
    {

        if (!CanDragDrop) return;

        Debug.WriteLine("Starting drop ---");
        _dragIndicator.IsVisible = false;
        _isDragging = false;
        _startDragPoint = default;

        if (_itemsHost == null)
        {
            throw new InvalidOperationException("Items host is null!");
        }

        if (DataContext is not IDragDropHandler context)
        {
            throw new InvalidOperationException("DataContext must be an IDragDropHandler");
        }

        if (e.Data.Get("item") is not IDragDropItem ddi)
        {
            throw new InvalidOperationException("Dragged item must be an IDragDropItem");
        }

        if (e.Data.Get("source") is not IList src)
        {
            throw new InvalidOperationException("Source must be an IList");
        }

        if (e.Source is not Control dropTarget)
        {
            throw new InvalidOperationException("idk what happened");
        }

        if (ddi == dropTarget.DataContext)
        {
            Debug.WriteLine($"DropItem is dropped on itself, returning...");
            return;
        }

        var dropTargetRoot = (Control?)dropTarget.GetVisualAncestors().FirstOrDefault(x => x.Parent == _itemsHost);

        if (dropTargetRoot == null)
        {
            Debug.WriteLine($"Dropping in parent list control, finding closest control...");

            // we dropped into the ItemsControl
            src.Remove(ddi);
            context.Drop(ddi, context.DropDestination, -1);
            // todo: find the items right above and after this and drop in the middle instead

            var siblings = _itemsHost.GetLogicalChildren().Where(x => ReferenceEquals(x.LogicalParent, _itemsHost)).Cast<Control>();
            var pointerInThis = e.GetPosition(this);

            dropTargetRoot = siblings.LastOrDefault(x => pointerInThis.Y > x.Bounds.Top && x.DataContext != ddi)!;
        }

        // we dropped over another item in the ItemsControl
        var dropTargetContext = dropTargetRoot.DataContext!;

        Debug.WriteLine($"Found {dropTargetContext} as drop target");

        var pointerPos = e.GetPosition(dropTargetRoot);
        var h = dropTargetRoot.Bounds.Height;

        // figure out which vertical half of the control we dropped the item to and decide the index based on that
        var bottom = pointerPos.Y > h / 2f;

        src.Remove(ddi);

        var idx = context.DropDestination.IndexOf(dropTargetContext);

        if (bottom)
            idx++;

        Debug.WriteLine($"Dropping {ddi} into {context} at {idx} ({(bottom ? "bottom" : "top")})");

        context.Drop(ddi, context.DropDestination, idx);

    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _container = e.NameScope.Get<Panel>("PART_Container");
        _itemsHost = e.NameScope.Get<ItemsControl>("PART_ItemsHost");
        _itemsHost.AddHandler(DragDrop.DropEvent, Drop);
        _itemsHost.AddHandler(DragDrop.DragOverEvent, DragOver);
        AddHandler(DragDrop.DragLeaveEvent, DragLeave);

        _container.Children.Add(_dragIndicator);
    }

    private void DragLeave(object? sender, DragEventArgs e)
    {
        if (!CanDragDrop) return;

        Debug.WriteLine("DragLeave ---");
        var ptr = e.GetPosition(this);
        if (this.Bounds.Contains(ptr))
        {

            Debug.WriteLine("Pointer still here, going on.");
            return;
        }

        Debug.WriteLine("Pointer left, resetting.");
        _startDragPoint = default;
        _dragIndicator.IsVisible = false;
        _isDragging = false;

        var parent = this.FindAncestorOfType<DragDropList>();

        if (parent != null)
        {
            parent.CanDragDrop = true;
        }
    }

    private Rectangle CreateSeparator()
    {
        return new Rectangle
        {
            Fill = Brushes.Orange,
            Margin = new Thickness(50, -1),
            IsHitTestVisible = false,
            IsVisible = false,
            RenderTransform = new TranslateTransform(),
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            //Width = 100,
            Height = 2
        };
    }

    public void Receive(DragOverMessage message)
    {
        if (!DragDrop.GetAllowDrop(this)) return;

        Debug.WriteLine("DragOverMessage ---");

        if (message.Sender != this)
        {
            Debug.WriteLine("Sender is not this, resetting.");
            _dragIndicator.IsVisible = false;
            _isDragging = false;
            _startDragPoint = default;

            var parent = this.FindAncestorOfType<DragDropList>();

            if (parent != null)
            {
                parent.CanDragDrop = true;
            }
        }
        else
        {
            Debug.WriteLine("Sender is this, skipping.");
        }
    }
}
