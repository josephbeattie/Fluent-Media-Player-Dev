using Rise.App.ViewModels;
using Rise.Common.Extensions.Markup;
using Rise.Data.Navigation;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class NavigationPage : Page
    {
        private NavigationDataSource NavDataSource => App.NavDataSource;
        private SettingsViewModel ViewModel => App.SViewModel;

        private readonly List<IconPack> IconPacks = new()
        {
            new(string.Empty, ResourceHelper.GetString("Default")),
            new("Colorful", ResourceHelper.GetString("Colorful"))
        };

        private readonly List<string> Show = new()
        {
            ResourceHelper.GetString("NoIcons"),
            ResourceHelper.GetString("OnlyIcons"),
            ResourceHelper.GetString("Everything")
        };

        private readonly List<string> Startup = new()
        {
            ResourceHelper.GetString("Home"),
            ResourceHelper.GetString("Playlists"),
            ResourceHelper.GetString("Songs"),
            ResourceHelper.GetString("Artists"),
            ResourceHelper.GetString("Albums"),
            ResourceHelper.GetString("LocalVideos"),
        };

        public NavigationPage()
        {
            InitializeComponent();

            InitializeNavigationExpanders();
        }

        private void IconPackComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var selected = IconPacks.FirstOrDefault(p => p.Id == ViewModel.IconPack);
            if (selected == null)
                IconPackComboBox.SelectedIndex = 0;
            else
                IconPackComboBox.SelectedItem = selected;
        }

        private void InitializeNavigationExpanders()
        {
            var items = NavDataSource.AllItems;

            GeneralItemsExpander.ItemsSource = items.Where(i => i.Group == "GeneralGroup");
            MusicItemsExpander.ItemsSource = items.Where(i => i.Group == "MusicGroup");
            VideoItemsExpander.ItemsSource = items.Where(i => i.Group == "VideosGroup");
        }

        private void GroupToggleSwitch_Loaded(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;

            toggle.IsOn = NavDataSource.IsGroupShown((string)toggle.Tag);
            toggle.Toggled += GroupToggleSwitch_Toggled;
        }

        private void ItemToggleSwitch_Loaded(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            toggle.Toggled += ItemToggleSwitch_Toggled;
        }
    }

    // Event handlers
    public sealed partial class NavigationPage
    {
        private void IconPackComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (IconPack)IconPackComboBox.SelectedItem;
            ViewModel.IconPack = selected.Id;
        }

        private void GroupToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            string group = (string)toggle.Tag;

            if (toggle.IsOn)
                NavDataSource.ShowGroup(group);
            else
                NavDataSource.HideGroup(group);
        }

        private void ItemToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            if (toggle.Tag is NavigationItemBase item)
                NavDataSource.ChangeItemVisibility(item, toggle.IsOn);
        }
    }
}
