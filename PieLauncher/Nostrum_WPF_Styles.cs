using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace PieLauncher;

public class Nostrum_WPF_Styles : RH
{
    public static ControlTemplate ComboBoxEditableTemplate => Get<ControlTemplate>("ComboBoxEditableTemplate");
    public static ControlTemplate ComboBoxTemplate => Get<ControlTemplate>("ComboBoxTemplate");
    public static ControlTemplate MenuItemControlTemplate1 => Get<ControlTemplate>("MenuItemControlTemplate1");
    public static DropShadowEffect BigDropShadow => Get<DropShadowEffect>("BigDropShadow");
    public static DropShadowEffect DropShadow => Get<DropShadowEffect>("DropShadow");
    public static DropShadowEffect FadedDropShadow => Get<DropShadowEffect>("FadedDropShadow");
    public static SolidColorBrush SelectionBackgroundBrush => Get<SolidColorBrush>("SelectionBackgroundBrush");
    public static SolidColorBrush SelectionBackgroundLightBrush => Get<SolidColorBrush>("SelectionBackgroundLightBrush");
    public static SolidColorBrush SelectionBorderBrush => Get<SolidColorBrush>("SelectionBorderBrush");
    public static Style ButtonContentOpacityStyle => Get<Style>("ButtonContentOpacityStyle");
    public static Style ButtonMainStyle => Get<Style>("ButtonMainStyle");
    public static Style ComboBoxEditableTextBox => Get<Style>("ComboBoxEditableTextBox");
    public static Style ComboBoxMainStyle => Get<Style>("ComboBoxMainStyle");
    public static Style ComboBoxToggleButton => Get<Style>("ComboBoxToggleButton");
    public static Style DefaultListItemStyle => Get<Style>("DefaultListItemStyle");
    public static Style EmptyFocusVisual => Get<Style>("EmptyFocusVisual");
    public static Style NoHilightListItemStyle => Get<Style>("NoHilightListItemStyle");
    public static Style NoHilightListItemStyleWithLines => Get<Style>("NoHilightListItemStyleWithLines");
}