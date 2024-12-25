using Rise.Common.Constants;
using Rise.Common.Extensions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Web
{
    public sealed partial class FeedbackPage : Page
    {
        public FeedbackPage()
        {
            InitializeComponent();
            TitleBar.SetTitleBarForCurrentView();
        }

        public static Task<bool> TryShowAsync()
            => ViewHelpers.OpenViewAsync<FeedbackPage>(minSize: new(380, 500));

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.NewIssue.LaunchAsync();
    }
}
