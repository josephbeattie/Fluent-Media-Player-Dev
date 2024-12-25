using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.Common.Attached;

/// <summary>
/// Attached properties for <see cref="FrameworkElement"/> objects.
/// </summary>
public sealed class FrameworkElementExtensions : DependencyObject
{
    /// <summary>
    /// A property that indicates whether the element's
    /// first ascendant is enabled.
    /// </summary>
    public static readonly DependencyProperty IsParentEnabledProperty =
        DependencyProperty.RegisterAttached("IsParentEnabled", typeof(bool),
            typeof(FrameworkElementExtensions), new(true, OnIsParentEnabledChanged));

    public static bool GetIsParentEnabled(FrameworkElement d)
        => (bool)d.GetValue(IsParentEnabledProperty);
    public static void SetIsParentEnabled(FrameworkElement d, bool value)
        => d.SetValue(IsParentEnabledProperty, value);

    private static void OnIsParentEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var elm = (FrameworkElement)d;
        if (!elm.IsLoaded)
            elm.Loaded += OnElementLoaded;
        else
            HandleParentEnabledChanged(elm, (bool)e.NewValue);
    }

    private static void OnElementLoaded(object sender, RoutedEventArgs e)
    {
        var elm = (FrameworkElement)sender;
        HandleParentEnabledChanged(elm, GetIsParentEnabled(elm));

        elm.Loaded -= OnElementLoaded;
    }

    private static void HandleParentEnabledChanged(FrameworkElement elm, bool enabled)
    {
        var parent = elm.FindAscendant<Control>();
        if (parent != null)
            parent.IsEnabled = enabled;
    }
}
