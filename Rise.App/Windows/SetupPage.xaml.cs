using Rise.App.Dialogs;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    /// <summary>
    /// Setup page.
    /// </summary>
    public sealed partial class SetupPage : Page
    {
        public SetupPage()
        {
            InitializeComponent();
            TitleBar.SetTitleBarForCurrentView();
        }

        private async void SetupButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Content = new SetupDialogContent(),
                FullSizeDesired = true,
            };

            dialog.Resources["ContentDialogMaxWidth"] = (double)762;
            dialog.Resources["ContentDialogMaxHeight"] = (double)490;

            _ = await dialog.ShowAsync();
        }
    }
}
