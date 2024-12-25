using Rise.App.ViewModels;
using Rise.Common.Extensions.Markup;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class SongLyricsPage : Page
    {
        private SongPropertiesViewModel Props { get; set; }

        public SongLyricsPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SongPropertiesViewModel props)
                Props = props;

            base.OnNavigatedTo(e);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            string lyrics = await Props.Model.GetLyricsAsync();
            if (!string.IsNullOrWhiteSpace(lyrics))
                Lyrics.Text = lyrics;
            else
                Lyrics.Text = ResourceHelper.GetString("NoLyricsFound");

            LoadingRing.IsActive = false;
            LoadingRing.Visibility = Visibility.Collapsed;
        }
    }
}
