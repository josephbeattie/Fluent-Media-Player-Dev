using Rise.Data.Json;
using Rise.Data.Messages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Dialogs
{
    public sealed partial class MessagesDialog : Page
    {
        private JsonBackendController<BasicNotification> NBackend
            => App.MViewModel.NBackend;

        public static readonly DependencyProperty SelectedNotificationProperty =
            DependencyProperty.Register(nameof(SelectedNotification), typeof(BasicNotification),
                typeof(MessagesDialog), null);

        public BasicNotification SelectedNotification
        {
            get => (BasicNotification)GetValue(SelectedNotificationProperty);
            set => SetValue(SelectedNotificationProperty, value);
        }

        public MessagesDialog()
        {
            InitializeComponent();
        }
    }

    // Event handlers
    public sealed partial class MessagesDialog : Page
    {
        private void MenuFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = NotificationsList.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedNotification = (BasicNotification)cont;
        }

        private async void DeleteMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedNotification != null)
            {
                NBackend.Items.Remove(SelectedNotification);
                await NBackend.SaveAsync();
            }
        }
    }
}
