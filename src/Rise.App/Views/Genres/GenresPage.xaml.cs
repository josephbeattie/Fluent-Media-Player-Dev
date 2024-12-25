using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Helpers;
using Rise.Data.Collections;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class GenresPage : MediaPageBase
    {
        public GenreViewModel SelectedItem
        {
            get => (GenreViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public GenresPage()
            : base("GenreName", SortDirection.Ascending, false, App.MViewModel.Genres)
        {
            InitializeComponent();
        }
    }

    // Event handlers
    public sealed partial class GenresPage
    {
        private void MainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is GenreViewModel genre && !KeyboardHelpers.IsCtrlPressed())
            {
                _ = Frame.Navigate(typeof(GenreSongsPage), genre.Model.Id);
            }
        }
    }
}
