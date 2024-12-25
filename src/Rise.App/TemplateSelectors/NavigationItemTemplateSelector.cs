using Rise.App.ViewModels;
using Rise.Data.Navigation;
using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.TemplateSelectors
{
    internal sealed class NavigationItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PlaylistTemplate { get; set; }

        public DataTemplate DestinationTemplate { get; set; }
        public DataTemplate HeaderTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }

        protected sealed override DataTemplate SelectTemplateCore(object item)
        {
            if (item is NavigationItemBase navItem)
            {
                return navItem.ItemType switch
                {
                    NavigationItemType.Header => HeaderTemplate,
                    NavigationItemType.Separator => SeparatorTemplate,
                    NavigationItemType.Destination => DestinationTemplate,
                    _ => throw new InvalidEnumArgumentException("The specified item type is invalid.", (int)navItem.ItemType, typeof(NavigationItemType))
                };
            }
            else if (item is PlaylistViewModel)
            {
                return PlaylistTemplate;
            }

            throw new ArgumentException("The item must be a playlist or a navigation item.", nameof(item));
        }
    }
}
