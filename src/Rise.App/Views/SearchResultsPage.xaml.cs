using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using Rise.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class SearchResultsPage : MediaPageBase
    {
        private IEnumerable<IGrouping<string, object>> GroupedItems;

        private readonly Dictionary<Type, string> ResourceNames = new()
        {
            { typeof(AlbumViewModel), "Albums" },
            { typeof(ArtistViewModel), "Artists" },
            { typeof(SongViewModel), "Songs" },
        };

        public SearchResultsPage()
            : base("SongTitle", SortDirection.Ascending, false, App.MViewModel.Songs)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            string search = e.NavigationParameter as string;
            string[] splitText = search.ToLower().Split(" ");

            string forFormat = ResourceHelper.GetString("ForX");
            ResultsFor.Text = string.Format(forFormat, search);

            var suitableItems = new List<object>();
            foreach (ArtistViewModel artist in App.MViewModel.Artists)
            {
                bool suitable = splitText.All((key) =>
                {
                    return artist.Name.ToLower().Contains(key);
                });

                if (suitable)
                    suitableItems.Add(artist);
            }

            MediaViewModel.Items.Filter = e => splitText.All((key) =>
            {
                return ((SongViewModel)e).Title.ToLower().Contains(key);
            });
            suitableItems.AddRange(MediaViewModel.Items);

            foreach (AlbumViewModel album in App.MViewModel.Albums)
            {
                bool suitable = splitText.All((key) =>
                {
                    return album.Title.ToLower().Contains(key);
                });

                if (suitable)
                    suitableItems.Add(album);
            }

            GroupedItems = suitableItems.GroupBy(e => ResourceNames[e.GetType()]);
        }
    }

    // Event handlers
    public sealed partial class SearchResultsPage
    {
        private void MainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is SongViewModel song)
                MediaViewModel.PlayFromItemCommand.Execute(song);
            else if (e.ClickedItem is AlbumViewModel album)
                _ = Frame.Navigate(typeof(AlbumSongsPage), album.Model.Id);
            else if (e.ClickedItem is ArtistViewModel artist)
                _ = Frame.Navigate(typeof(ArtistSongsPage), artist.Model.Id);
        }
    }

    public sealed class SearchResultTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SongTemplate { get; set; }
        public DataTemplate AlbumTemplate { get; set; }
        public DataTemplate ArtistTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is AlbumViewModel)
                return AlbumTemplate;
            else if (item is ArtistViewModel)
                return ArtistTemplate;

            return SongTemplate;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
