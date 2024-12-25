using Rise.Data.ViewModels;
using Rise.Models;

namespace Rise.App.ViewModels
{
    public sealed class SearchItemViewModel : ViewModel<SearchItem>
    {
        public SearchItemViewModel(SearchItem model = null)
        {
            if (model != null)
            {
                Model = model;
            }
            else
            {
                Model = new SearchItem();
            }
        }

        public string Title
        {
            get => Model.Title;
            set
            {
                if (value != Model.Title)
                {
                    Model.Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string Subtitle
        {
            get => Model.Subtitle;
            set
            {
                if (value != Model.Subtitle)
                {
                    Model.Subtitle = value;
                    OnPropertyChanged(nameof(Subtitle));
                }
            }
        }

        public string ItemType
        {
            get => Model.ItemType;
            set
            {
                if (value != Model.ItemType)
                {
                    Model.ItemType = value;
                    OnPropertyChanged(nameof(ItemType));
                }
            }
        }

        public string Thumbnail
        {
            get => Model.Thumbnail;
            set
            {
                if (value != Model.Thumbnail)
                {
                    Model.Thumbnail = value;
                    OnPropertyChanged(nameof(Thumbnail));
                }
            }
        }
    }
}
