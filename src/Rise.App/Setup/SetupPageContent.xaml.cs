using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Rise.App.Setup
{
    /// <summary>
    /// A control that hosts the contents necessary for
    /// a page in the RiseMP setup.
    /// </summary>
    [ContentProperty(Name = nameof(PageContent))]
    public sealed partial class SetupPageContent : UserControl
    {
        public static readonly DependencyProperty HeaderProperty
            = DependencyProperty.Register(nameof(Header), typeof(string),
                typeof(SetupPageContent), null);
        /// <summary>
        /// The page's header, a persistent bit of title
        /// text.
        /// </summary>
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon), typeof(object),
                typeof(SetupPageContent), null);
        /// <summary>
        /// Icon to show in this page, will automatically hide
        /// on small window sizes.
        /// </summary>
        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty PageContentProperty
            = DependencyProperty.Register(nameof(PageContent), typeof(object),
                typeof(SetupPageContent), null);
        /// <summary>
        /// Content to show in this page.
        /// </summary>
        public object PageContent
        {
            get => GetValue(PageContentProperty);
            set => SetValue(PageContentProperty, value);
        }

        public static readonly DependencyProperty ShowIconProperty
            = DependencyProperty.Register(nameof(ShowIcon), typeof(bool),
                typeof(SetupPageContent), null);
        /// <summary>
        /// Whether to show the page's icon. If false, a compact
        /// layout will be used.
        /// </summary>
        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        public static readonly DependencyProperty IsBackButtonAutoPaddingEnabledProperty
            = DependencyProperty.Register(nameof(IsBackButtonAutoPaddingEnabled), typeof(bool),
                typeof(SetupPageContent), new PropertyMetadata(true));
        /// <summary>
        /// Whether to take into account a back button when applying
        /// padding, true by default.
        /// </summary>
        public bool IsBackButtonAutoPaddingEnabled
        {
            get => (bool)GetValue(IsBackButtonAutoPaddingEnabledProperty);
            set => SetValue(IsBackButtonAutoPaddingEnabledProperty, value);
        }

        public SetupPageContent()
        {
            InitializeComponent();
        }
    }
}
