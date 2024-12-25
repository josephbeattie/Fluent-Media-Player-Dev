using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Data.Collections;
using System;
using System.Linq;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Rise.App.Views
{
    public sealed partial class GenreSongsPage : MediaPageBase
    {
        private MainViewModel MViewModel => App.MViewModel;
        public SongViewModel SelectedItem
        {
            get => (SongViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private GenreViewModel SelectedGenre;

        private CompositionPropertySet _propSet;
        private SpriteVisual _backgroundVisual;

        public GenreSongsPage()
            : base(App.MViewModel.Playlists)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;

            PlaylistHelper.AddPlaylistsToSubItem(AddTo, AddSelectedItemToPlaylistCommand);
            PlaylistHelper.AddPlaylistsToFlyout(AddToBar, AddMediaItemsToPlaylistCommand);
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is Guid id)
            {
                SelectedGenre = MViewModel.Genres.
                    FirstOrDefault(g => g.Model.Id == id);
            }
            else if (e.NavigationParameter is string str)
            {
                SelectedGenre = MViewModel.Genres.
                    FirstOrDefault(g => g.Name == str);
            }

            CreateViewModel("SongTitle", SortDirection.Ascending, false, IsGenre, MViewModel.Songs);
            bool IsGenre(object s)
                => ((SongViewModel)s).Genres == SelectedGenre.Name;
        }

        private void OnMainListLoaded(object sender, RoutedEventArgs e)
        {
            var surface = LoadedImageSurface.StartLoadFromUri(new("ms-appx:///Assets/BlankGenre.png"));
            (_propSet, _backgroundVisual) = MainList.CreateParallaxGradientVisual(surface, BackgroundHost);
        }
    }

    // Event handlers
    public sealed partial class GenreSongsPage
    {
        private void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
                MediaViewModel.PlayFromItemCommand.Execute(song);
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainList.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (SongViewModel)cont;
        }

        private void BackgroundHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_backgroundVisual == null) return;
            _backgroundVisual.Size = new Vector2((float)e.NewSize.Width, (float)BackgroundHost.Height);
        }
    }
}
