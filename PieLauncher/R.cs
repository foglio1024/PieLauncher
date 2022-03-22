//////////////////////////////////////////////////////////////
//// File automatically generated from PieLauncher.csproj ////
//////////////////////////////////////////////////////////////

using Nostrum.WPF.Converters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace PieLauncher.R
{
	// Resources/Resources.xaml
	public class Resources : RH
	{
		public static Color AccentGradient1 => Get<Color>("AccentGradient1");
		public static Color AccentGradient2 => Get<Color>("AccentGradient2");
		public static Color BaseGradient1 => Get<Color>("BaseGradient1");
		public static Color BaseGradient2 => Get<Color>("BaseGradient2");
		public static Color BaseGradient3 => Get<Color>("BaseGradient3");
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
		public static Geometry SvgSettingsRounded => Get<Geometry>("SvgSettingsRounded");
		public static GradientStopCollection DefaultGradientStops => Get<GradientStopCollection>("DefaultGradientStops");
		public static LinearGradientBrush DayGradient => Get<LinearGradientBrush>("DayGradient");
		public static LinearGradientBrush DuskGradient => Get<LinearGradientBrush>("DuskGradient");
		public static LinearGradientBrush NightGradient => Get<LinearGradientBrush>("NightGradient");
		public static LinearGradientBrush SunriseGradient => Get<LinearGradientBrush>("SunriseGradient");
		public static RadialGradientBrush DefaultBackgroundBrush => Get<RadialGradientBrush>("DefaultBackgroundBrush");
		public static RadialGradientBrush RingBackgroundBrush => Get<RadialGradientBrush>("RingBackgroundBrush");
	}

	// pack://application:,,,/Nostrum.WPF;component/Resources/Converters.xaml
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

	// pack://application:,,,/Nostrum.WPF;component/Resources/MiscResources.xaml
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

	// pack://application:,,,/Nostrum.WPF;component/Resources/SVG.xaml
	public class Nostrum_WPF_SVG : RH
	{
		public static StreamGeometry SvgAchievements => Get<StreamGeometry>("SvgAchievements");
		public static StreamGeometry SvgAdd => Get<StreamGeometry>("SvgAdd");
		public static StreamGeometry SvgAddCircle => Get<StreamGeometry>("SvgAddCircle");
		public static StreamGeometry SvgAddUser => Get<StreamGeometry>("SvgAddUser");
		public static StreamGeometry SvgAddUsers => Get<StreamGeometry>("SvgAddUsers");
		public static StreamGeometry SvgAuto => Get<StreamGeometry>("SvgAuto");
		public static StreamGeometry SvgAwaken => Get<StreamGeometry>("SvgAwaken");
		public static StreamGeometry SvgBasket => Get<StreamGeometry>("SvgBasket");
		public static StreamGeometry SvgBlock => Get<StreamGeometry>("SvgBlock");
		public static StreamGeometry SvgBlurOff => Get<StreamGeometry>("SvgBlurOff");
		public static StreamGeometry SvgBlurOn => Get<StreamGeometry>("SvgBlurOn");
		public static StreamGeometry SvgChatMessage => Get<StreamGeometry>("SvgChatMessage");
		public static StreamGeometry SvgCheckAll => Get<StreamGeometry>("SvgCheckAll");
		public static StreamGeometry SvgCheckCircle => Get<StreamGeometry>("SvgCheckCircle");
		public static StreamGeometry SvgClose => Get<StreamGeometry>("SvgClose");
		public static StreamGeometry SvgCollapseAll => Get<StreamGeometry>("SvgCollapseAll");
		public static StreamGeometry SvgConfirm => Get<StreamGeometry>("SvgConfirm");
		public static StreamGeometry SvgCopy => Get<StreamGeometry>("SvgCopy");
		public static StreamGeometry SvgCrossedSwords => Get<StreamGeometry>("SvgCrossedSwords");
		public static StreamGeometry SvgDelegateLeader => Get<StreamGeometry>("SvgDelegateLeader");
		public static StreamGeometry SvgDisband => Get<StreamGeometry>("SvgDisband");
		public static StreamGeometry SvgDiscord => Get<StreamGeometry>("SvgDiscord");
		public static StreamGeometry SvgDotsVertical => Get<StreamGeometry>("SvgDotsVertical");
		public static StreamGeometry SvgDownArrow => Get<StreamGeometry>("SvgDownArrow");
		public static StreamGeometry SvgDrag => Get<StreamGeometry>("SvgDrag");
		public static StreamGeometry SvgExpandAll => Get<StreamGeometry>("SvgExpandAll");
		public static StreamGeometry SvgEye => Get<StreamGeometry>("SvgEye");
		public static StreamGeometry SvgFolder => Get<StreamGeometry>("SvgFolder");
		public static StreamGeometry SvgGift => Get<StreamGeometry>("SvgGift");
		public static StreamGeometry SvgGitHub => Get<StreamGeometry>("SvgGitHub");
		public static StreamGeometry SvgGuild => Get<StreamGeometry>("SvgGuild");
		public static StreamGeometry SvgHeart => Get<StreamGeometry>("SvgHeart");
		public static StreamGeometry SvgHide => Get<StreamGeometry>("SvgHide");
		public static StreamGeometry SvgInfo => Get<StreamGeometry>("SvgInfo");
		public static StreamGeometry SvgMail => Get<StreamGeometry>("SvgMail");
		public static StreamGeometry SvgMapMarker => Get<StreamGeometry>("SvgMapMarker");
		public static StreamGeometry SvgMatching => Get<StreamGeometry>("SvgMatching");
		public static StreamGeometry SvgMenuRight => Get<StreamGeometry>("SvgMenuRight");
		public static StreamGeometry SvgMinimize => Get<StreamGeometry>("SvgMinimize");
		public static StreamGeometry SvgMoney => Get<StreamGeometry>("SvgMoney");
		public static StreamGeometry SvgMove => Get<StreamGeometry>("SvgMove");
		public static StreamGeometry SvgOpenLink => Get<StreamGeometry>("SvgOpenLink");
		public static StreamGeometry SvgPaypal => Get<StreamGeometry>("SvgPaypal");
		public static StreamGeometry SvgPen => Get<StreamGeometry>("SvgPen");
		public static StreamGeometry SvgPin => Get<StreamGeometry>("SvgPin");
		public static StreamGeometry SvgQuestionMark => Get<StreamGeometry>("SvgQuestionMark");
		public static StreamGeometry SvgQuestLog => Get<StreamGeometry>("SvgQuestLog");
		public static StreamGeometry SvgReload => Get<StreamGeometry>("SvgReload");
		public static StreamGeometry SvgRemoveCircle => Get<StreamGeometry>("SvgRemoveCircle");
		public static StreamGeometry SvgRemoveUser => Get<StreamGeometry>("SvgRemoveUser");
		public static StreamGeometry SvgSearch => Get<StreamGeometry>("SvgSearch");
		public static StreamGeometry SvgSettings => Get<StreamGeometry>("SvgSettings");
		public static StreamGeometry SvgShare => Get<StreamGeometry>("SvgShare");
		public static StreamGeometry SvgShirt => Get<StreamGeometry>("SvgShirt");
		public static StreamGeometry SvgShop => Get<StreamGeometry>("SvgShop");
		public static StreamGeometry SvgSocial => Get<StreamGeometry>("SvgSocial");
		public static StreamGeometry SvgStar => Get<StreamGeometry>("SvgStar");
		public static StreamGeometry SvgTrophy => Get<StreamGeometry>("SvgTrophy");
		public static StreamGeometry SvgTwitch => Get<StreamGeometry>("SvgTwitch");
		public static StreamGeometry SvgUnpin => Get<StreamGeometry>("SvgUnpin");
		public static StreamGeometry SvgUpArrow => Get<StreamGeometry>("SvgUpArrow");
		public static StreamGeometry SvgUser => Get<StreamGeometry>("SvgUser");
		public static StreamGeometry SvgUserAdd => Get<StreamGeometry>("SvgUserAdd");
		public static StreamGeometry SvgUsers => Get<StreamGeometry>("SvgUsers");
		public static StreamGeometry SvgWarning => Get<StreamGeometry>("SvgWarning");
	}

	// pack://application:,,,/Nostrum.WPF;component/Resources/Styles.xaml
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

	public class RH
	{
		protected static T Get<T>(string res)
		{
			return (T)Application.Current.FindResource(res);
		}
	}
}
