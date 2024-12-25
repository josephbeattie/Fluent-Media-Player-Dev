using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Storage;

namespace Rise.App.ViewModels
{
    public sealed partial class VideoViewModel : ViewModel<Video>, IMediaItem
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoViewModel"/>
        /// class that wraps a <see cref="Video"/> object.
        /// </summary>
        public VideoViewModel(Video model = null)
        {
            Model = model ?? new Video();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the video title.
        /// </summary>
        public string Title
        {
            get => Model.Title;
            set
            {
                if (Model.Title != value)
                {
                    Model.Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        /// <summary>
        /// Gets or sets the video length.
        /// </summary>
        public TimeSpan Length
        {
            get => Model.Length;
            set
            {
                if (Model.Length != value)
                {
                    Model.Length = value;
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        /// <summary>
        /// Gets or sets the video directors.
        /// </summary>
        public string Directors
        {
            get => Model.Directors;
            set
            {
                if (Model.Directors != value)
                {
                    Model.Directors = value;
                    OnPropertyChanged(nameof(Directors));
                }
            }
        }

        /// <summary>
        /// Gets or sets the video location.
        /// </summary>
        public string Location
        {
            get => Model.Location;
            set
            {
                if (Model.Location != value)
                {
                    Model.Location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }

        /// <summary>
        /// Gets or sets the video thumbnail.
        /// </summary>
        public string Thumbnail
        {
            get => Model.Thumbnail;
            set
            {
                if (Model.Thumbnail != value)
                {
                    Model.Thumbnail = value;
                    OnPropertyChanged(nameof(Thumbnail));
                }
            }
        }

        /// <summary>
        /// Gets or sets the video year.
        /// </summary>
        public uint Year
        {
            get => Model.Year;
            set
            {
                if (Model.Year != value)
                {
                    Model.Year = value;
                    OnPropertyChanged(nameof(Year));
                }
            }
        }

        /// <summary>
        /// Gets or sets the video rating.
        /// </summary>
        public uint Rating
        {
            get => Model.Rating;
            set
            {
                if (Model.Rating != value)
                {
                    Model.Rating = value;
                    OnPropertyChanged(nameof(Rating));
                }
            }
        }

        /// <inheritdoc />
        public bool IsLocal
        {
            get => Model.IsLocal;
            set
            {
                if (value != Model.IsLocal)
                {
                    Model.IsLocal = value;
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
            if (!App.MViewModel.Videos.Contains(this))
            {
                App.MViewModel.Videos.Add(this);
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

        /// <summary>
        /// Deletes item data from the backend.
        /// </summary>
        public async Task DeleteAsync(bool queue = false)
        {
            if (App.MViewModel.Videos.Contains(this))
            {
                App.MViewModel.Videos.Remove(this);

                if (queue)
                    NewRepository.Repository.QueueRemove(Model);
                else
                    await NewRepository.Repository.DeleteAsync(Model);
            }
        }
        #endregion

        #region Editing
        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            Model = await NewRepository.Repository.GetItemAsync<Video>(Model.Id);
        }
        #endregion

        #region Playback
        /// <summary>
        /// Creates a <see cref="MediaPlaybackItem"/> from this <see cref="VideoViewModel"/>.
        /// </summary>
        /// <returns>A <see cref="MediaPlaybackItem"/> based on the video.</returns>
        public async Task<MediaPlaybackItem> AsPlaybackItemAsync()
        {
            var uri = new Uri(Location);
            if (uri.IsFile)
            {
                var file = await StorageFile.GetFileFromPathAsync(Location);
                return await file.GetVideoAsync();
            }

            return await WebHelpers.GetVideoFromUriAsync(uri, Title, Directors, Thumbnail);
        }
        #endregion
    }

    public static class VideoViewModelExtensions
    {
        public static Task<VideoViewModel> AsVideoAsync(this MediaPlaybackItem item)
        {
            var displayProps = item.GetDisplayProperties();

            var video = new VideoViewModel
            {
                Title = displayProps.VideoProperties.Title,
                Directors = item.Source.CustomProperties["Artists"] as string,
                Location = item.Source.Uri.ToString(),
                Year = (uint)item.Source.CustomProperties["Year"],
                Length = (TimeSpan)item.Source.Duration,
                Thumbnail = URIs.VideoThumb
            };

            return Task.FromResult(video);
        }
    }
}
