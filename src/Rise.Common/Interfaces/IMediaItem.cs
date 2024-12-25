using System.Threading.Tasks;
using Windows.Media.Playback;

namespace Rise.Common.Interfaces
{
    /// <summary>
    /// Defines a simple contract for classes that encapsulate
    /// playable media.
    /// </summary>
    public interface IMediaItem
    {
        /// <summary>
        /// Path to the item. This is simply an URI
        /// stored as a string.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Whether the item is local or online.
        /// </summary>
        bool IsLocal { get; }

        /// <summary>
        /// Creates a <see cref="MediaPlaybackItem"/> representation
        /// of this item.
        /// </summary>
        /// <returns>A Task that represents the conversion
        /// operation.</returns>
        Task<MediaPlaybackItem> AsPlaybackItemAsync();
    }
}
