using Rise.Common;
using Rise.Common.Extensions.Markup;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public sealed class ArtistViewModel : ViewModel<Artist>
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ArtistViewModel class that wraps an Artist object.
        /// </summary>
        public ArtistViewModel(Artist model = null)
        {
            Model = model ?? new Artist();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the artist name.
        /// </summary>
        public string Name
        {
            get
            {
                if (Model.Name == "UnknownArtistResource")
                    return ResourceHelper.GetString("UnknownArtistResource");
                return Model.Name;
            }
            set
            {
                if (value != Model.Name)
                {
                    Model.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the artist picture.
        /// </summary>
        public string Picture
        {
            get => Model.Picture;
            set
            {
                if (value != Model.Picture)
                {
                    Model.Picture = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the artist's song count.
        /// </summary>
        public int SongCount
            => App.MViewModel.Songs.Count(s => s.Model.Artist == Model.Name);
        public string LocalizedSongCount
            => ResourceHelper.GetLocalizedCount("Song", SongCount);

        /// <summary>
        /// Gets the artist's album count.
        /// </summary>
        public int AlbumCount
            => App.MViewModel.Albums.Count(a => a.Model.Artist == Model.Name);
        public string LocalizedAlbumCount
            => ResourceHelper.GetLocalizedCount("Album", AlbumCount);

        public string LocalizedSongsAndAlbums
            => $"{LocalizedSongCount}, {LocalizedAlbumCount}";
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync(bool queue = false)
        {
            if (!App.MViewModel.Artists.Contains(this))
            {
                App.MViewModel.Artists.Add(this);
            }

            if (queue)
            {
                NewRepository.Repository.QueueUpsert(Model);
            }
            else
            {
                await NewRepository.Repository.UpsertAsync(Model);
            }
        }
        #endregion
    }
}
