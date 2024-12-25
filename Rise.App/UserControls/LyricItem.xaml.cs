using Rise.Common.Extensions;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.UserControls
{
    public sealed partial class LyricItem : UserControl
    {
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public LyricItem()
        {
            InitializeComponent();
        }
    }

    // Dependency properties
    public partial class LyricItem
    {
        public static readonly DependencyProperty IsSelectedProperty
            = DependencyProperty.Register(nameof(IsSelected), typeof(bool),
                typeof(LyricItem), new PropertyMetadata(false, OnSelectedPropertyChanged));

        public static readonly DependencyProperty TextProperty
            = DependencyProperty.Register(nameof(Text), typeof(string),
                typeof(LyricItem), new PropertyMetadata(string.Empty));
    }

    // Property changed handlers
    public partial class LyricItem
    {
        private static void OnSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LyricItem item)
            {
                var selected = (bool)e.NewValue;
                _ = VisualStateManager.GoToState(item, selected ? "Selected" : "NotSelected", true);
            }
        }
    }
}
