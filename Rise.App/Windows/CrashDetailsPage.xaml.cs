using Rise.Common.Constants;
using Rise.Common.Extensions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    /// <summary>
    /// A page that shows details about crashes when the app is
    /// relaunched after a crash happening.
    /// </summary>
    public sealed partial class CrashDetailsPage : Page
    {
        private string Text;

        public CrashDetailsPage()
        {
            InitializeComponent();
            TitleBar.SetTitleBarForCurrentView();
        }

        public static Task<bool> TryShowAsync(string crashDetails)
            => ViewHelpers.OpenViewAsync<CrashDetailsPage>(crashDetails, new(500, 500));

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Text = e.Parameter as string;
            base.OnNavigatedTo(e);
        }
    }

    // Event handlers
    public sealed partial class CrashDetailsPage
    {
        private void SubmitIssueButton_Click(object sender, RoutedEventArgs e)
            => _ = URLs.Feedback.LaunchAsync();
    }
}
