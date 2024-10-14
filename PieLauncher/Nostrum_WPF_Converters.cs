using Nostrum.WPF.Converters;

namespace PieLauncher;

public class Nostrum_WPF_Converters : RH
{
    public static BooleanInverter BoolInverter => Get<BooleanInverter>("BoolInverter");
    public static BoolToVisibility BoolToVisibility => Get<BoolToVisibility>("BoolToVisibility");
    public static ColorToTransparent ColorToTransparent => Get<ColorToTransparent>("ColorToTransparent");
    public static DurationToStringConverter DurationToString => Get<DurationToStringConverter>("DurationToString");
    public static EnumDescriptionConverter EnumDescriptionConverter => Get<EnumDescriptionConverter>("EnumDescriptionConverter");
    public static EpochConverter EpochConverter => Get<EpochConverter>("EpochConverter");
    public static FactorToAngleConverter FactorToAngle => Get<FactorToAngleConverter>("FactorToAngle");
    public static ListBoxItemIndexConverter ListBoxItemIndex => Get<ListBoxItemIndexConverter>("ListBoxItemIndex");
    public static MathMultiplicationConverter MathMultiplication => Get<MathMultiplicationConverter>("MathMultiplication");
    public static NullToVisibilityConverter NullToVisibility => Get<NullToVisibilityConverter>("NullToVisibility");
    public static RoundedClipConverter RoundedClipConverter => Get<RoundedClipConverter>("RoundedClipConverter");
    public static ValueToFactorConverter ValueToFactor => Get<ValueToFactorConverter>("ValueToFactor");
}