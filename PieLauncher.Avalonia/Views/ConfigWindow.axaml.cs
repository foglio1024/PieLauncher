using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;

using PieLauncher.Core.ViewModels;

namespace PieLauncher.Avalonia.Views;

using System.Diagnostics;

public class NodesTreeViewDropHandler : DropHandlerBase
{
    private bool Validate<T>(TreeView treeView, DragEventArgs e, object? sourceContext, object? targetContext, bool bExecute) where T : PieItemBase
    {
        if (sourceContext is not T sourceNode
            //|| targetContext is not T targetNode
            || treeView.GetVisualAt(e.GetPosition(treeView)) is not Control { DataContext: T targetNode }
            )
        {
            return false;
        }

        var sourceParent = sourceNode.Parent;
        var targetParent = targetNode.Parent;

        Debug.WriteLine($"Source is {sourceNode.Name} ({sourceNode.GetType().Name}) | Target is {targetNode.Name} ({targetNode.GetType().Name})");

        var sourceNodes = (sourceParent as FolderViewModel)?.Apps;
        var targetNodes = (targetParent as FolderViewModel)?.Apps;

        var droppingIntoItself = targetNodes == (sourceContext as FolderViewModel)?.Apps;

        var nestingTooMuch = sourceContext is FolderViewModel && targetParent is not FolderViewModel { Parent: null };

        if (sourceNodes is null || targetNodes is null || droppingIntoItself || nestingTooMuch)
        {
            return false;
        }

        var sourceIndex = sourceNodes.IndexOf(sourceNode);
        var targetIndex = targetNodes.IndexOf(targetNode);

        if (sourceIndex < 0 || targetIndex < 0 || e.DragEffects != DragDropEffects.Move)
        {
            return false;
        }

        if (!bExecute)
        {
            return true;
        }

        if (sourceNodes == targetNodes)
        {
            MoveItem(sourceNodes, sourceIndex, targetIndex);
        }
        else
        {
            MoveItem(sourceNodes, targetNodes, sourceIndex, targetIndex);
        }

        return true;

    }

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is TreeView treeView)
        {
            return Validate<PieItemBase>(treeView, e, sourceContext, targetContext, false);
        }
        return false;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is TreeView treeView)
        {
            return Validate<PieItemBase>(treeView, e, sourceContext, targetContext, true);
        }
        return false;
    }
}

public partial class ConfigWindow : Window
{
    //private IList? _dragSource;
    //Line _dropLine;

    public ConfigWindow()
    {
        InitializeComponent();

        //AddHandler(DragDrop.DragOverEvent, DragOver);
        //AddHandler(DragDrop.DropEvent, Drop);

        //_dropLine = new Line()
        //{
        //    Stroke = Brushes.Orange,
        //    StrokeThickness = 2,
        //    Width = 100,
        //    Height = 2,
        //    HorizontalAlignment = HorizontalAlignment.Center
        //};
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {

    }

    private void OnTreeViewKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is not Key.Delete) return;
        if (e.Source is Control { DataContext: PieItemBase item and { Parent: FolderViewModel folder } })
        {
            folder.Apps.Remove(item);
        }
    }
    // -------------------- Do all this stuff in attached behaviors -----------------------
    //

    //private void Drop(object? sender, DragEventArgs e)
    //{
    //    return;

    //    var data = e.Data.Get("item") as IDragDropItem;

    //    if (data is null)
    //    {
    //        return;
    //    }

    //    if (e.Source is not Control ctrl) return;

    //    var ic = ctrl.FindAncestorOfType<ItemsControl>();
    //    if (ic == null || !DragDrop.GetAllowDrop(ic)) return;
    //    var icc = ic.Parent as Panel;

    //    if (ic.DataContext is not IDragDropHandler vm) return;

    //    var dropTarget = ctrl.DataContext;
    //    var src = ((IList)ic.ItemsSource!);
    //    var newidx = src.IndexOf(dropTarget);

    //    if (dropTarget == ic.DataContext)
    //    {
    //        // we dropped in an empty place of the folder: find the position

    //        var h = ic.Bounds.Height;

    //        var pointerPos = e.GetPosition(ic);
    //        Control? actualTarget = null;
    //        foreach (var icItem in ic.GetVisualDescendants().OfType<Control>().Where((x => x.Tag == "x")))
    //        {
    //            var ch = icItem.Bounds.Height + icItem.TranslatePoint(new Point(), ic).Value.Y;
    //            if (ch > pointerPos.Y)
    //            {
    //                break;
    //            }
    //            actualTarget = icItem;
    //        }
    //        if (!icc.Children.Contains(_dropLine))
    //        {
    //            icc.Children.Add(_dropLine);
    //        }
    //        _dropLine.Margin = new Thickness(0, icc.TranslatePoint(new Point(0, 0), actualTarget).Value.Y, 0, 0);
    //        newidx = src.IndexOf(actualTarget.DataContext) + 1;
    //    }

    //    var oldidx = src.IndexOf(data);
    //    vm.Drop(data, ic.ItemsSource, newidx);
    //    Debug.WriteLine($"DROP: {ctrl.DataContext} @ {newidx}");

    //    var offset = newidx < oldidx && newidx != -1 ? 1 : 0;

    //    _dragSource?.RemoveAt(oldidx + offset);
    //    _dragSource = null;
    //}

    //private void DragOver(object? sender, DragEventArgs e)
    //{
    //    return;

    //    if (e.Source is not Control ctrl) return;
    //    var ic = ctrl.FindAncestorOfType<ItemsControl>();
    //    if (ic == null || !DragDrop.GetAllowDrop(ic)) return;
    //    if (ic.DataContext is not IDragDropHandler vm) return;

    //    e.DragEffects = DragDropEffects.Move;

    //    var data = e.Data.Get("item") as IDragDropItem;
    //    if (data is null) return;

    //    if (!vm.CanDrop(data, ic.ItemsSource))
    //    {
    //        e.DragEffects = DragDropEffects.None;
    //    }

    //    Debug.WriteLine($"DRAGOVER: {e.Source}");
    //}

    //private async void PieItem_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    //{
    //    return;
    //    //if (DataContext is not IDragDropHandler vm) return;
    //    if (sender is not Control { DataContext: PieItemBase item } element) return;

    //    var ic = element.FindLogicalAncestorOfType<ItemsControl>();

    //    if (ic is not null && ic.DataContext is IDragDropHandler vm)
    //    {
    //        vm.StartDrag(item);

    //        var dragData = new DataObject();
    //        dragData.Set("item", item);
    //        _dragSource = (IList?)element.FindLogicalAncestorOfType<ItemsControl>()?.ItemsSource;
    //        var result = await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);

    //        //Debug.WriteLine($"DragAndDrop result: {result}");
    //    }
    //}

    // ------------------------------------------------------------------------------------
}