using Rise.App.ViewModels;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Rise.App.UserControls
{
    /// <summary>
    /// A control that displays the data of a
    /// <see cref="SongViewModel"/>.
    /// </summary>
    public sealed partial class SongData : UserControl
    {
        public static readonly DependencyProperty GoToAlbumCommandProperty
            = DependencyProperty.Register(nameof(GoToAlbumCommand), typeof(ICommand),
                typeof(SongData), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command to execute when a song's
        /// album is clicked.
        /// </summary>
        public ICommand GoToAlbumCommand
        {
            get => (ICommand)GetValue(GoToAlbumCommandProperty);
            set => SetValue(GoToAlbumCommandProperty, value);
        }

        public static readonly DependencyProperty GoToArtistCommandProperty
            = DependencyProperty.Register(nameof(GoToArtistCommand), typeof(ICommand),
                typeof(SongData), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command to execute when a song's
        /// artist is clicked.
        /// </summary>
        public ICommand GoToArtistCommand
        {
            get => (ICommand)GetValue(GoToArtistCommandProperty);
            set => SetValue(GoToArtistCommandProperty, value);
        }

        public static readonly DependencyProperty GoToGenreCommandProperty
            = DependencyProperty.Register(nameof(GoToGenreCommand), typeof(ICommand),
                typeof(SongData), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command to execute when a song's
        /// genre is clicked.
        /// </summary>
        public ICommand GoToGenreCommand
        {
            get => (ICommand)GetValue(GoToGenreCommandProperty);
            set => SetValue(GoToGenreCommandProperty, value);
        }

        public static readonly DependencyProperty SongProperty
            = DependencyProperty.Register(nameof(Song), typeof(SongViewModel),
                typeof(SongData), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the song to show.
        /// </summary>
        public SongViewModel Song
        {
            get => (SongViewModel)GetValue(SongProperty);
            set => SetValue(SongProperty, value);
        }

        public static readonly DependencyProperty ThumbnailCornerRadiusProperty
            = DependencyProperty.Register(nameof(ThumbnailCornerRadius), typeof(CornerRadius),
                typeof(SongData), new PropertyMetadata(new CornerRadius(6)));

        /// <summary>
        /// Gets or sets the corner radius for the album art.
        /// </summary>
        public CornerRadius ThumbnailCornerRadius
        {
            get => (CornerRadius)GetValue(ThumbnailCornerRadiusProperty);
            set => SetValue(ThumbnailCornerRadiusProperty, value);
        }

        public static readonly DependencyProperty ShowTrackNumberProperty
            = DependencyProperty.Register(nameof(ShowTrackNumber), typeof(bool),
                typeof(SongData), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the song's
        /// track number should be shown.
        /// </summary>
        public bool ShowTrackNumber
        {
            get => (bool)GetValue(ShowTrackNumberProperty);
            set => SetValue(ShowTrackNumberProperty, value);
        }

        public static readonly DependencyProperty ShowThumbnailProperty
            = DependencyProperty.Register(nameof(ShowThumbnail), typeof(bool),
                typeof(SongData), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the song's
        /// thumbnail should be shown.
        /// </summary>
        public bool ShowThumbnail
        {
            get => (bool)GetValue(ShowThumbnailProperty);
            set => SetValue(ShowThumbnailProperty, value);
        }

        public static readonly DependencyProperty ShowAlbumProperty
            = DependencyProperty.Register(nameof(ShowAlbum), typeof(bool),
                typeof(SongData), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the song's
        /// album should be shown.
        /// </summary>
        public bool ShowAlbum
        {
            get => (bool)GetValue(ShowAlbumProperty);
            set => SetValue(ShowAlbumProperty, value);
        }

        public static readonly DependencyProperty ShowArtistProperty
            = DependencyProperty.Register(nameof(ShowArtist), typeof(bool),
                typeof(SongData), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the song's
        /// artist should be shown.
        /// </summary>
        public bool ShowArtist
        {
            get => (bool)GetValue(ShowArtistProperty);
            set => SetValue(ShowArtistProperty, value);
        }

        public static readonly DependencyProperty ShowLengthProperty
            = DependencyProperty.Register(nameof(ShowLength), typeof(bool),
                typeof(SongData), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the song's
        /// length should be shown.
        /// </summary>
        public bool ShowLength
        {
            get => (bool)GetValue(ShowLengthProperty);
            set => SetValue(ShowLengthProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the song's
        /// genre should be shown.
        /// </summary>
        public bool ShowGenre
        {
            get => (bool)GetValue(ShowGenreProperty);
            set => SetValue(ShowGenreProperty, value);
        }

        public static readonly DependencyProperty ShowGenreProperty
            = DependencyProperty.Register(nameof(ShowGenre), typeof(bool),
                typeof(SongData), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a the control's side content.
        /// </summary>
        public object SideControlContent
        {
            get => GetValue(SideControlContentProperty);
            set => SetValue(SideControlContentProperty, value);
        }

        public static readonly DependencyProperty SideControlContentProperty
            = DependencyProperty.Register(nameof(SideControlContent), typeof(object),
                typeof(SongData), new PropertyMetadata(null));

        public static readonly DependencyProperty PlayCommandProperty
            = DependencyProperty.Register(nameof(PlayCommand), typeof(ICommand),
                typeof(SongData), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command to execute when the play
        /// button is clicked.
        /// </summary>
        public ICommand PlayCommand
        {
            get => (ICommand)GetValue(PlayCommandProperty);
            set => SetValue(PlayCommandProperty, value);
        }

        public static readonly DependencyProperty EditCommandProperty
            = DependencyProperty.Register(nameof(EditCommand), typeof(ICommand),
                typeof(SongData), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command to execute when the edit
        /// button is clicked.
        /// </summary>
        public ICommand EditCommand
        {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public SongData()
        {
            InitializeComponent();
        }
    }

    // Event handlers
    public sealed partial class SongData
    {
        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }
    }
}
