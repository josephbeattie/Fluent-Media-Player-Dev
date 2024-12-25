using Rise.App.ViewModels;
using Rise.Common.Extensions.Markup;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class MediaLibraryPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        private readonly List<string> Deletion = new()
        {
            ResourceHelper.GetString("RemoveFromApp"),
            ResourceHelper.GetString("RemoveFromDevice")
        };

        public MediaLibraryPage()
        {
            InitializeComponent();
        }

        private void GotoManage_Click(object sender, RoutedEventArgs e)
        {
            AllSettingsPage.Current.MainSettingsHeader.Text = ResourceHelper.GetString("/Settings/MediaLibraryManageFoldersTitle");
            AllSettingsPage.Current.MainSettingsHeaderIcon.Glyph = "\uE838";
            AllSettingsPage.Current.SettingsMainFrame.Navigate(typeof(MediaSourcesPage));
        }
    }
}
