using Rise.Common.Constants;
using Rise.Common.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class InsiderPage : Page
    {
        public InsiderPage()
        {
            InitializeComponent();
        }
        private void ExpanderControl_Click(object sender, RoutedEventArgs e)
        {
            _ = Frame.Navigate(typeof(InsiderWallpapers));
        }
    }
}
