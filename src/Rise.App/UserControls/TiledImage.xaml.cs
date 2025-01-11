using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.UserControls
{
    public sealed partial class TiledImage : UserControl
    {
        public TiledImage()
        {
            InitializeComponent();
        }

        private static readonly DependencyProperty BackgroundUriProperty =
            DependencyProperty.Register("BackgroundUri", typeof(string), typeof(TiledImage), null);

        public string BackgroundUri
        {
            get => (string)GetValue(BackgroundUriProperty);
            set => SetValue(BackgroundUriProperty, value);
        }

        private static readonly DependencyProperty IconUriProperty =
            DependencyProperty.Register("IconUri", typeof(string), typeof(TiledImage), null);

        public string IconUri
        {
            get => (string)GetValue(IconUriProperty);
            set => SetValue(IconUriProperty, value);
        }

        private static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(TiledImage), null);

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }
    }
}
