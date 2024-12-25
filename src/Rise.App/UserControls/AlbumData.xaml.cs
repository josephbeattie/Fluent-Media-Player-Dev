using Rise.App.ViewModels;
using Rise.Common.Enums;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.UserControls
{
    /// <summary>
    /// A control that displays the data of an
    /// <see cref="AlbumViewModel"/>.
    /// </summary>
    public sealed partial class AlbumData : UserControl
    {
        public static readonly DependencyProperty GoToArtistCommandProperty
            = DependencyProperty.Register(nameof(GoToArtistCommand), typeof(ICommand),
                typeof(AlbumData), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command to execute when an album's
        /// artist is clicked.
        /// </summary>
        public ICommand GoToArtistCommand
        {
            get => (ICommand)GetValue(GoToArtistCommandProperty);
            set => SetValue(GoToArtistCommandProperty, value);
        }

        public static readonly DependencyProperty AlbumProperty
            = DependencyProperty.Register(nameof(Album), typeof(AlbumViewModel),
                typeof(AlbumData), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the album to show.
        /// </summary>
        public AlbumViewModel Album
        {
            get => (AlbumViewModel)GetValue(AlbumProperty);
            set => SetValue(AlbumProperty, value);
        }

        public static readonly DependencyProperty ViewModeProperty
            = DependencyProperty.Register(nameof(ViewMode), typeof(AlbumViewMode),
                typeof(AlbumData), new PropertyMetadata(AlbumViewMode.VerticalTile, OnViewModeChanged));

        /// <summary>
        /// Gets or sets the way the album should be displayed.
        /// </summary>
        public AlbumViewMode ViewMode
        {
            get => (AlbumViewMode)GetValue(ViewModeProperty);
            set => SetValue(ViewModeProperty, value);
        }

        public static readonly DependencyProperty ThumbnailCornerRadiusProperty
            = DependencyProperty.Register(nameof(ThumbnailCornerRadius), typeof(CornerRadius),
                typeof(AlbumData), new PropertyMetadata(new CornerRadius(8)));

        /// <summary>
        /// Gets or sets the corner radius for the album art.
        /// </summary>
        public CornerRadius ThumbnailCornerRadius
        {
            get => (CornerRadius)GetValue(ThumbnailCornerRadiusProperty);
            set => SetValue(ThumbnailCornerRadiusProperty, value);
        }

        public static readonly DependencyProperty ShowThumbnailProperty
            = DependencyProperty.Register(nameof(ShowThumbnail), typeof(bool),
                typeof(AlbumData), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the album's
        /// thumbnail should be shown.
        /// </summary>
        public bool ShowThumbnail
        {
            get => (bool)GetValue(ShowThumbnailProperty);
            set => SetValue(ShowThumbnailProperty, value);
        }

        public static readonly DependencyProperty ShowTitleProperty
            = DependencyProperty.Register(nameof(ShowTitle), typeof(bool),
                typeof(AlbumData), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the album's
        /// title should be shown.
        /// </summary>
        public bool ShowTitle
        {
            get => (bool)GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, value);
        }

        public static readonly DependencyProperty ShowArtistProperty
            = DependencyProperty.Register(nameof(ShowArtist), typeof(bool),
                typeof(AlbumData), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the album's
        /// artist should be shown.
        /// </summary>
        public bool ShowArtist
        {
            get => (bool)GetValue(ShowArtistProperty);
            set => SetValue(ShowArtistProperty, value);
        }

        public static readonly DependencyProperty ShowGenresProperty
            = DependencyProperty.Register(nameof(ShowGenres), typeof(bool),
                typeof(AlbumData), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the album's
        /// genres should be shown.
        /// </summary>
        public bool ShowGenres
        {
            get => (bool)GetValue(ShowGenresProperty);
            set => SetValue(ShowGenresProperty, value);
        }

        public static readonly DependencyProperty ShowReleaseYearProperty
            = DependencyProperty.Register(nameof(ShowReleaseYear), typeof(bool),
                typeof(AlbumData), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the album's
        /// release year should be shown.
        /// </summary>
        public bool ShowReleaseYear
        {
            get => (bool)GetValue(ShowReleaseYearProperty);
            set => SetValue(ShowReleaseYearProperty, value);
        }

        public AlbumData()
        {
            InitializeComponent();
        }
    }

    // Event handlers
    public sealed partial class AlbumData
    {
        private static void OnViewModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AlbumData ctrl)
            {
                var mode = (AlbumViewMode)e.NewValue;
                _ = VisualStateManager.GoToState(ctrl, mode.ToString(), true);
            }
        }
    }
}
