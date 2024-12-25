using Rise.App.ViewModels;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class SongFilePage : Page
    {
        private SongPropertiesViewModel Props { get; set; }

        public SongFilePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SongPropertiesViewModel props)
            {
                Props = props;
            }

            base.OnNavigatedTo(e);
        }

        private async void OpenFileLocation_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _ = await Launcher.LaunchFolderPathAsync(Props.Location.Replace(Props.Filename, string.Empty));
        }
    }
}
