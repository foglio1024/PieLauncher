using System.Windows.Media;

namespace PieLauncher;

public class Resources : RH
{
    public static Color AccentGradient1 => Get<Color>("AccentGradient1");
    public static Color AccentGradient2 => Get<Color>("AccentGradient2");
    public static Color DuskGradient1 => Get<Color>("DuskGradient1");
    public static Color DuskGradient2 => Get<Color>("DuskGradient2");
    public static Color NightGradient1 => Get<Color>("NightGradient1");
    public static Color NightGradient2 => Get<Color>("NightGradient2");
    public static Color SunriseGradient1 => Get<Color>("SunriseGradient1");
    public static Color SunriseGradient2 => Get<Color>("SunriseGradient2");
    public static FontFamily K2DRegular => Get<FontFamily>("K2DRegular");
    public static Geometry SvgAddRounded => Get<Geometry>("SvgAddRounded");
    public static Geometry SvgArrowDown => Get<Geometry>("SvgArrowDown");
    public static Geometry SvgArrowLeft => Get<Geometry>("SvgArrowLeft");
    public static Geometry SvgArrowRight => Get<Geometry>("SvgArrowRight");
    public static Geometry SvgArrowUp => Get<Geometry>("SvgArrowUp");
    public static Geometry SvgCloseRounded => Get<Geometry>("SvgCloseRounded");
    public static Geometry SvgDeleteRounded => Get<Geometry>("SvgDeleteRounded");
    public static Geometry SvgDrag => Get<Geometry>("SvgDrag");
    public static Geometry SvgExport => Get<Geometry>("SvgExport");
    public static Geometry SvgFileRounded => Get<Geometry>("SvgFileRounded");
    public static Geometry SvgFolderRounded => Get<Geometry>("SvgFolderRounded");
    public static Geometry SvgHorizontalRule => Get<Geometry>("SvgHorizontalRule");
    public static Geometry SvgImage => Get<Geometry>("SvgImage");
    public static Geometry SvgImport => Get<Geometry>("SvgImport");
    public static Geometry SvgLabel => Get<Geometry>("SvgLabel");
    public static Geometry SvgLink => Get<Geometry>("SvgLink");
    public static Geometry SvgMoreHorizontal => Get<Geometry>("SvgMoreHorizontal");
    public static Geometry SvgNext => Get<Geometry>("SvgNext");
    public static Geometry SvgPause => Get<Geometry>("SvgPause");
    public static Geometry SvgPlay => Get<Geometry>("SvgPlay");
    public static Geometry SvgPrev => Get<Geometry>("SvgPrev");
    public static Geometry SvgSave => Get<Geometry>("SvgSave");
    public static Geometry SvgSearch => Get<Geometry>("SvgSearch");
    public static Geometry SvgSearchImage => Get<Geometry>("SvgSearchImage");
    public static Geometry SvgSecurityRounded => Get<Geometry>("SvgSecurityRounded");
    public static Geometry SvgSettingsRounded => Get<Geometry>("SvgSettingsRounded");
    public static GradientStopCollection DefaultGradientStops => Get<GradientStopCollection>("DefaultGradientStops");
    public static GradientStopCollection LightGradientStops => Get<GradientStopCollection>("LightGradientStops");
    public static LinearGradientBrush DayGradient => Get<LinearGradientBrush>("DayGradient");
    public static LinearGradientBrush DuskGradient => Get<LinearGradientBrush>("DuskGradient");
    public static LinearGradientBrush NightGradient => Get<LinearGradientBrush>("NightGradient");
    public static LinearGradientBrush SunriseGradient => Get<LinearGradientBrush>("SunriseGradient");
    public static RadialGradientBrush BackgroundBrush => Get<RadialGradientBrush>("BackgroundBrush");
    public static RadialGradientBrush LightBackgroundBrush => Get<RadialGradientBrush>("LightBackgroundBrush");
    public static RadialGradientBrush RingBackgroundBrush => Get<RadialGradientBrush>("RingBackgroundBrush");
    public static SolidColorBrush DefaultBorderBrush => Get<SolidColorBrush>("DefaultBorderBrush");
    public static SolidColorBrush DefaultTextBrush => Get<SolidColorBrush>("DefaultTextBrush");
    public static SolidColorBrush DimTextBrush => Get<SolidColorBrush>("DimTextBrush");
    public static SolidColorBrush DisabledTextBrush => Get<SolidColorBrush>("DisabledTextBrush");
    public static SolidColorBrush OverlayBrush => Get<SolidColorBrush>("OverlayBrush");
}