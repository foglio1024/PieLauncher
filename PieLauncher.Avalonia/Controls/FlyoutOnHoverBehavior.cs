using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace PieLauncher.Avalonia.Controls;

public abstract class FlyoutOnHoverBehavior : AvaloniaObject
{
    public static readonly AttachedProperty<bool> EnabledProperty =
        AvaloniaProperty.RegisterAttached<FlyoutOnHoverBehavior, Interactive, bool>("Enabled");

    private static readonly Dictionary<Control, (IDisposable Handler, Control Owner)> _handlers = [];

    static FlyoutOnHoverBehavior()
    {
        EnabledProperty.Changed.AddClassHandler<Interactive>(EnabledChanged);
    }

    private static void EnabledChanged(Interactive elem, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue is true)
        {
            elem.RemoveHandler(InputElement.PointerEnteredEvent, Handler);
            elem.AddHandler(InputElement.PointerEnteredEvent, Handler);
            //interactElem.RemoveHandler(InputElement.PointerExitedEvent, Handler);
            //interactElem.AddHandler(InputElement.PointerExitedEvent, Handler);
        }
        else
        {
            elem.RemoveHandler(InputElement.PointerEnteredEvent, Handler);
        }
    }

    private static void Handler(object? s, PointerEventArgs e)
    {
        if (s is not Control owner)
        {
            return;
        }

        if (FlyoutBase.GetAttachedFlyout(owner) is not Flyout { Content: Control content }) return;

        var handler = content.AddDisposableHandler(InputElement.PointerExitedEvent, HandlePointerExit);

        _handlers[content] = (handler, owner);

        FlyoutBase.ShowAttachedFlyout(owner);
    }

    private static void HandlePointerExit(object? sender, PointerEventArgs e)
    {
        if (sender is not Control content) return;
        if (!_handlers.TryGetValue(content, out var item)) return;

        HideFlyout(content, item.Handler, item.Owner);
    }

    private static void HideFlyout(Control content, IDisposable handler, Control owner)
    {
        handler.Dispose();
        var flyout = FlyoutBase.GetAttachedFlyout(owner);
        flyout?.Hide();

        _handlers.Remove(content);
    }

    public static void SetEnabled(AvaloniaObject element, bool value)
    {
        element.SetValue(EnabledProperty, value);
    }

    public static bool? GetEnabled(AvaloniaObject element)
    {
        return element.GetValue(EnabledProperty);
    }

    public static void ForceClose()
    {
        foreach (var (content, (handler, owner)) in _handlers)
        {
            HideFlyout(content, handler, owner);
        }
    }
}