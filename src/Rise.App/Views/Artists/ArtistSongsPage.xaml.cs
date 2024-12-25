using Rise.App.Converters;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using Rise.Data.Collections;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Globalization;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Web.Http;

namespace Rise.App.Views
{
    public sealed partial class ArtistSongsPage : MediaPageBase
    {
        private SettingsViewModel SViewModel => App.SViewModel;

        public SongViewModel SelectedItem
        {
            get => (SongViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private ArtistViewModel SelectedArtist;

        // These handle the way artist biography is displayed
        private bool ShowingSummarized = true;
        private string ShortBio;
        private string LongBio;

        private CompositionPropertySet _propSet;
        private SpriteVisual _backgroundVisual;

        public ArtistSongsPage()
            : base(App.MViewModel.Playlists)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;

            PlaylistHelper.AddPlaylistsToSubItem(AddToList, AddSelectedItemToPlaylistCommand);
            PlaylistHelper.AddPlaylistsToFlyout(AddToBar, AddMediaItemsToPlaylistCommand);
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is Guid id)
            {
                SelectedArtist = App.MViewModel.Artists.
                    FirstOrDefault(a => a.Model.Id == id);
            }
            else if (e.NavigationParameter is string str)
            {
                SelectedArtist = App.MViewModel.Artists.
                    FirstOrDefault(a => a.Name == str);
            }

            CreateViewModel("GSongAlbum|SongDisc|SongTrack", SortDirection.Ascending, false, IsFromArtist, App.MViewModel.Songs);
            bool IsFromArtist(object s)
            {
                var song = (SongViewModel)s;
                return song.Artist == SelectedArtist.Name || song.AlbumArtist == SelectedArtist.Name;
            }
        }

        private void OnMainListLoaded(object sender, RoutedEventArgs e)
        {
            var surface = LoadedImageSurface.StartLoadFromUri(new(SelectedArtist.Picture));
            (_propSet, _backgroundVisual) = MainList.CreateParallaxGradientVisual(surface, BackgroundHost);
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            ArtistDuration.Text = await Task.Run(() => TimeSpanToString.GetShortFormat(TimeSpan.FromSeconds(MediaViewModel.Items.Cast<SongViewModel>().Select(s => s.Length).Aggregate((t, t1) => t + t1).TotalSeconds)));

            string name = SelectedArtist.Name;
            if (!SViewModel.FetchOnlineData ||
                !WebHelpers.IsInternetAccessAvailable() ||
                name == ResourceHelper.GetString("UnknownArtistResource"))
            {
                VisualStateManager.GoToState(this, "LastFMUnavailableState", true);
            }
            else
            {
                string genre = await GetGenreFromArtistAsync(name);
                SongAlbums.Text += $" • {genre}";

                TopTracks.ItemsSource = await GetTopTracksAsync(name);
                NoListeners.Text = await GetMonthlyListenersAsync(name);

                ShortBio = await GetArtistBioAsync(name, true);
                AboutArtist.Text = ShortBio;

                if (string.IsNullOrWhiteSpace(ShortBio))
                    VisualStateManager.GoToState(this, "ArtistBioUnavailableState", true);
            }
        }
    }

    // Event handlers
    public sealed partial class ArtistSongsPage
    {
        private void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
                MediaViewModel.PlayFromItemCommand.Execute(song);
        }

        private void SongFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainList.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (SongViewModel)cont;
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnSong.IsOpen = true;
        }

        private async void ReadMoreAbout_Click(object sender, RoutedEventArgs e)
        {
            if (ShowingSummarized)
            {
                if (string.IsNullOrEmpty(LongBio))
                    LongBio = await GetArtistBioAsync(SelectedArtist.Name, false);

                AboutArtist.Text = LongBio;
                ReadMoreAbout.Content = ResourceHelper.GetString("ReadLess");
            }
            else
            {
                AboutArtist.Text = ShortBio;
                ReadMoreAbout.Content = ResourceHelper.GetString("ReadMore");
            }

            ShowingSummarized = !ShowingSummarized;
        }


        private void BackgroundHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_backgroundVisual == null) return;
            _backgroundVisual.Size = new Vector2((float)e.NewSize.Width, (float)BackgroundHost.Height);
        }

        private async Task<List<Track>> GetTopTracksAsync(string artist)
        {
            try
            {
                string lfmUrl = URLs.LastFM + "artist.gettoptracks&artist=" + artist + "&api_key=" + LastFM.Key + "&limit=8";
                using (var client = new HttpClient())
                {
                    var result = await client.TryGetStringAsync(new(lfmUrl));
                    if (result.Succeeded)
                    {
                        StringReader strReader = new(result.Value);
                        XmlSerializer serializer = new(typeof(LFM));
                        XmlTextReader xmlReader = new(strReader);

                        var lfm = (LFM)serializer.Deserialize(xmlReader);
                        return lfm.Toptracks.Track;
                    }
                }
            }
            catch { }

            return null;
        }

        private async Task<string> GetGenreFromArtistAsync(string artist)
        {
            try
            {
                var textInfo = new CultureInfo(ApplicationLanguages.PrimaryLanguageOverride).TextInfo;
                string lfmUrl = URLs.LastFM + "artist.getinfo&artist=" + artist + "&api_key=" + LastFM.Key;

                using (var client = new HttpClient())
                {
                    var res = await client.TryGetStringAsync(new(lfmUrl));
                    if (res.Succeeded)
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(res.Value);

                        var node = doc.DocumentElement.SelectSingleNode("/lfm/artist/tags/tag/name");
                        if (node != null)
                            return textInfo.ToTitleCase(node.InnerText);
                    }
                }
            }
            catch { }

            return ResourceHelper.GetString("UnknownGenreResource");
        }

        private async Task<string> GetArtistBioAsync(string artist, bool summarized)
        {
            try
            {
                var textInfo = new CultureInfo(ApplicationLanguages.PrimaryLanguageOverride).TextInfo;
                string lfmUrl = URLs.LastFM + "artist.getinfo&artist=" + artist + "&api_key=" + LastFM.Key + "&autocorrect=1";

                using (var client = new HttpClient())
                {
                    var res = await client.TryGetStringAsync(new(lfmUrl));
                    if (res.Succeeded)
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(res.Value);

                        XmlNode node;
                        if (summarized)
                            node = doc.DocumentElement.SelectSingleNode("/lfm/artist/bio/summary");
                        else
                            node = doc.DocumentElement.SelectSingleNode("/lfm/artist/bio/content");

                        if (node != null)
                        {
                            var url = doc.DocumentElement.SelectSingleNode("/lfm/artist/url");
                            var noUrl = node.InnerText.
                                Replace("<a href=\"" + url.InnerText + "\">Read more on Last.fm</a>", string.Empty);

                            if (summarized)
                                return noUrl;

                            return noUrl.Replace(". User-contributed text is available under the Creative Commons By-SA License; additional terms may apply.", string.Empty);
                        }
                    }
                }
            }
            catch { }

            return ResourceHelper.GetString("NoArtistInfo");
        }

        private async Task<string> GetMonthlyListenersAsync(string artist)
        {
            try
            {
                var textInfo = new CultureInfo(ApplicationLanguages.PrimaryLanguageOverride).TextInfo;
                string lfmUrl = URLs.LastFM + "artist.getinfo&artist=" + artist + "&api_key=" + LastFM.Key;

                using (var client = new HttpClient())
                {
                    var res = await client.TryGetStringAsync(new(lfmUrl));
                    if (res.Succeeded)
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(res.Value);

                        var node = doc.DocumentElement.SelectSingleNode("/lfm/artist/stats/listeners");
                        if (node != null && long.TryParse(node.InnerText, out long num))
                            return string.Format(ResourceHelper.GetString("NListeners"), FormatNumber.Format(num));
                    }
                }
            }
            catch { }

            return string.Empty;
        }
    }
}
