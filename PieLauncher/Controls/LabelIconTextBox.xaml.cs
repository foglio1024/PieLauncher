using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PieLauncher.Controls
{
    public partial class LabelIconTextBox
    {


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LabelIconTextBox), new PropertyMetadata(""));


        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(LabelIconTextBox), new PropertyMetadata(""));

        public PathGeometry SvgIcon
        {
            get { return (PathGeometry)GetValue(SvgIconProperty); }
            set { SetValue(SvgIconProperty, value); }
        }

        public static readonly DependencyProperty SvgIconProperty = DependencyProperty.Register("SvgIcon", typeof(PathGeometry), typeof(LabelIconTextBox), new PropertyMetadata(null));



        public LabelIconTextBox()
        {
            InitializeComponent();
        }
    }
}
