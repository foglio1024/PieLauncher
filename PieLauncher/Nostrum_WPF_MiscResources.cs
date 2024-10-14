using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PieLauncher;

public class Nostrum_WPF_MiscResources : RH
{
    public static QuadraticEase QuadraticEase => Get<QuadraticEase>("QuadraticEase");
    public static RotateTransform DefaultRotateTransform => Get<RotateTransform>("DefaultRotateTransform");
    public static RotateTransform Rotate45 => Get<RotateTransform>("Rotate45");
    public static RotateTransform Rotate45Inv => Get<RotateTransform>("Rotate45Inv");
    public static ScaleTransform DefaultScaleTransform => Get<ScaleTransform>("DefaultScaleTransform");
    public static SkewTransform Skew45 => Get<SkewTransform>("Skew45");
    public static SkewTransform Skew45Inv => Get<SkewTransform>("Skew45Inv");
    public static TranslateTransform DefaultTranslateTransform => Get<TranslateTransform>("DefaultTranslateTransform");
}