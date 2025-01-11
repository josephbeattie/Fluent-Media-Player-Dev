﻿using Rise.Common.Extensions.Markup;
using Rise.Data.ViewModels;
using Rise.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public sealed class AlbumViewModel : ViewModel<Album>
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the AlbumViewModel class that wraps an Album object.
        /// </summary>
        public AlbumViewModel(Album model = null)
        {
            Model = model ?? new Album();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the album title.
        /// </summary>
        public string Title
        {
            get => Model.Title == "UnknownAlbumResource" ? ResourceHelper.GetString("UnknownAlbumResource") : Model.Title;
            set
            {
                if (value != Model.Title)
                {
                    Model.Title = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the album artist.
        /// </summary>
        public string Artist
        {
            get => Model.Artist == "UnknownArtistResource" ? ResourceHelper.GetString("UnknownArtistResource") : Model.Artist;
            set
            {
                if (value != Model.Artist)
                {
                    Model.Artist = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the album title + artist together.
        /// </summary>
        public string TitleWithArtist => $"{Title} - {Artist}";

        /// <summary>
        /// Gets or sets the album genre.
        /// </summary>
        public string Genres
        {
            get => Model.Genres == "UnknownGenreResource" ? ResourceHelper.GetString("UnknownGenreResource") : Model.Genres;
            set
            {
                if (value != Model.Genres)
                {
                    Model.Genres = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the album thumbnail.
        /// </summary>
        public string Thumbnail
        {
            get => Model.Thumbnail;
            set
            {
                if (value != Model.Thumbnail)
                {
                    Model.Thumbnail = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or setss the album release year.
        /// </summary>
        public uint Year
        {
            get => Model.Year;
            set
            {
                if (value != Model.Year)
                {
                    Model.Year = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(LocalizedYear));
                }
            }
        }

        public string LocalizedYear
        {
            get
            {
                string year = ResourceHelper.GetString("ReleaseYearN");
                return Year == 0 ? string.Format(year, ResourceHelper.GetString("Unknown")) : string.Format(year, Year);
            }
        }

        public int TrackCount
            => App.MViewModel.Songs.Count(s => s.Album == Model.Title);
        public string LocalizedTrackCount
            => ResourceHelper.GetLocalizedCount("Song", TrackCount);
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync(bool queue = false)
        {
            if (!App.MViewModel.Albums.Contains(this))
            {
                App.MViewModel.Albums.Add(this);
            }

            if (queue)
            {
                _ = NewRepository.Repository.QueueUpsert(Model);
            }
            else
            {
                _ = await NewRepository.Repository.UpsertAsync(Model);
            }
        }
        #endregion

        #region Editing
        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            Model = await NewRepository.Repository.GetItemAsync<Album>(Model.Id);
        }
        #endregion
    }
}
