using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.TemplateSelectors
{
    /// <summary>
    /// A type safe base class for template selectors. Allows
    /// specifying a type for the item parameter in the base
    /// SelectTemplateCore overrides.
    /// </summary>
    internal abstract class TemplateSelectorBase<TItem> : DataTemplateSelector
    {
        protected sealed override DataTemplate SelectTemplateCore(object item)
            => SelectTemplateCore((TItem)item);

        protected sealed override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore((TItem)item, container);

        protected virtual DataTemplate SelectTemplateCore(TItem item)
            => base.SelectTemplateCore(item);
    }

    /// <summary>
    /// A type safe base class for template selectors. Allows
    /// specifying a type for the item and container parameters
    /// in the base SelectTemplateCore overrides.
    /// </summary>
    internal abstract class TemplateSelectorBase<TItem, TContainer> : DataTemplateSelector
        where TContainer : DependencyObject
    {
        protected sealed override DataTemplate SelectTemplateCore(object item)
            => SelectTemplateCore((TItem)item);

        protected sealed override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore((TItem)item, (TContainer)container);

        protected virtual DataTemplate SelectTemplateCore(TItem item)
            => base.SelectTemplateCore(item);

        protected virtual DataTemplate SelectTemplateCore(TItem item, TContainer container)
            => base.SelectTemplateCore(item, container);
    }
}
