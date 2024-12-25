using Rise.Common.Extensions.Markup;
using Rise.Data.ViewModels;
using Rise.Models;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public sealed class GenreViewModel : ViewModel<Genre>
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the AlbumViewModel class that wraps an Album object.
        /// </summary>
        public GenreViewModel(Genre model = null)
        {
            Model = model ?? new Genre();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the genre name.
        /// </summary>
        public string Name
        {
            get
            {
                if (Model.Name == "UnknownGenreResource")
                    return ResourceHelper.GetString("UnknownGenreResource");
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
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync(bool queue = false)
        {
            if (!App.MViewModel.Genres.Contains(this))
            {
                App.MViewModel.Genres.Add(this);
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
