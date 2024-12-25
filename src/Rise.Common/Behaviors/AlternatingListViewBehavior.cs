using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Xaml.Interactivity;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

#nullable enable

namespace Rise.Common.Behaviors;

public sealed class AlternatingListViewBehavior : Behavior<ListViewBase>
{
    // On Win11, we have access to an extra border to apply the brushes without
    // needing a CornerRadius of 0
    private readonly bool HasContract14 = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 14);

    public static readonly DependencyProperty AlternateBackgroundProperty = DependencyProperty.Register(
        nameof(AlternateBackground),
        typeof(Brush),
        typeof(AlternatingListViewBehavior),
        new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty AlternateBorderThicknessProperty = DependencyProperty.Register(
        nameof(AlternateBorderThickness),
        typeof(Thickness),
        typeof(AlternatingListViewBehavior),
        new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty AlternateBorderBrushProperty = DependencyProperty.Register(
        nameof(AlternateBorderBrush),
        typeof(Brush),
        typeof(AlternatingListViewBehavior),
        new PropertyMetadata(default(Brush?)));

    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
        nameof(Background),
        typeof(Brush),
        typeof(AlternatingListViewBehavior),
        new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
        nameof(BorderThickness),
        typeof(Thickness),
        typeof(AlternatingListViewBehavior),
        new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
        nameof(BorderBrush),
        typeof(Brush),
        typeof(AlternatingListViewBehavior),
        new PropertyMetadata(default(Brush?)));

    public Brush? AlternateBorderBrush
    {
        get => (Brush?)GetValue(AlternateBorderBrushProperty);
        set => SetValue(AlternateBorderBrushProperty, value);
    }

    public Thickness AlternateBorderThickness
    {
        get => (Thickness)GetValue(AlternateBorderThicknessProperty);
        set => SetValue(AlternateBorderThicknessProperty, value);
    }

    public Brush? AlternateBackground
    {
        get => (Brush?)GetValue(AlternateBackgroundProperty);
        set => SetValue(AlternateBackgroundProperty, value);
    }

    public Brush? BorderBrush
    {
        get => (Brush?)GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }

    public Thickness BorderThickness
    {
        get => (Thickness)GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    public Brush? Background
    {
        get => (Brush?)GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.ActualThemeChanged += OnActualThemeChanged;
        AssociatedObject.ContainerContentChanging += OnContainerContentChanging;

        if (AssociatedObject.Items != null)
            AssociatedObject.Items.VectorChanged += ItemsOnVectorChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject.ActualThemeChanged -= OnActualThemeChanged;
        AssociatedObject.ContainerContentChanging -= OnContainerContentChanging;

        if (AssociatedObject.Items != null)
            AssociatedObject.Items.VectorChanged -= ItemsOnVectorChanged;
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (AssociatedObject.Items == null) return;
        for (uint i = 0; i < AssociatedObject.Items.Count; i++)
        {
            if (AssociatedObject.ContainerFromIndex((int)i) is not SelectorItem itemContainer)
                continue;

            UpdateAlternateLayout(itemContainer, i);
        }
    }

    private void ItemsOnVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs args)
    {
        // If the index is at the end we can ignore
        if (args.Index == (sender.Count - 1))
            return;

        // Only need to handle Inserted and Removed because we'll handle everything else in the
        // OnContainerContentChanging method
        if (args.CollectionChange is CollectionChange.ItemInserted or CollectionChange.ItemRemoved)
        {
            for (uint i = args.Index; i < sender.Count; i++)
            {
                if (AssociatedObject.ContainerFromIndex((int)i) is not SelectorItem itemContainer)
                    continue;

                UpdateAlternateLayout(itemContainer, i);
            }
        }
    }

    private void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (args.Phase > 0 || args.InRecycleQueue || args.ItemIndex < 0) return;
        UpdateAlternateLayout(args.ItemContainer, (uint)args.ItemIndex);
    }

    private void UpdateAlternateLayout(SelectorItem itemContainer, uint itemIndex)
    {
        if (HasContract14)
            UpdateAlternateLayoutContract14(itemContainer, itemIndex);
        else
            UpdateAlternateLayoutNoContract14(itemContainer, itemIndex);
    }

    // For Windows 11 onwards
    private void UpdateAlternateLayoutContract14(SelectorItem itemContainer, uint itemIndex)
    {
        var border = itemContainer.FindDescendant<Border>();
        if (itemIndex % 2 == 0)
        {
            itemContainer.Background = AlternateBackground;
            if (border != null)
            {
                border.Background = AlternateBackground;
                border.BorderBrush = AlternateBorderBrush;
                border.BorderThickness = AlternateBorderThickness;
            }
        }
        else
        {
            itemContainer.Background = Background;
            if (border != null)
            {
                border.Background = Background;
                border.BorderBrush = BorderBrush;
                border.BorderThickness = BorderThickness;
            }
        }
    }

    // For Windows 10
    private void UpdateAlternateLayoutNoContract14(SelectorItem itemContainer, uint itemIndex)
    {
        if (itemIndex % 2 == 0)
        {
            itemContainer.Background = AlternateBackground;
            itemContainer.BorderBrush = AlternateBorderBrush;
            itemContainer.BorderThickness = AlternateBorderThickness;
        }
        else
        {
            itemContainer.Background = Background;
            itemContainer.BorderBrush = BorderBrush;
            itemContainer.BorderThickness = BorderThickness;
        }
    }
}