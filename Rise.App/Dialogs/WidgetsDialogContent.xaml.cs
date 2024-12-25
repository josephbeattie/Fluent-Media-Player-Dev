using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetsDialogContent : Page
    {
        public WidgetsDialogContent()
        {
            this.InitializeComponent();
            if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                WidgetsImage.Visibility = Visibility.Collapsed;
                WidgetsImageDark.Visibility = Visibility.Visible;
            }
            else
            {
                WidgetsImageDark.Visibility = Visibility.Collapsed;
                WidgetsImage.Visibility = Visibility.Visible;
            }
        }
    }
}
