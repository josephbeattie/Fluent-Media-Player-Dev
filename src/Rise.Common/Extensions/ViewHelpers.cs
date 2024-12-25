using Rise.Common.Threading;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.Common.Extensions
{
    public static class ViewHelpers
    {
        /// <summary>
        /// Creates a new <see cref="ApplicationView"/> which hosts a frame
        /// that navigates to <typeparamref name="TPage"/> with the provided
        /// parameter, then shows it.
        /// </summary>
        public static async Task<bool> OpenViewAsync<TPage>(object parameter = null, Size minSize = default, bool useMinSize = true)
            where TPage : Page
        {
            ApplicationView view = await CreateViewAsync<TPage>(parameter);
            bool shown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(view.Id);

            if (shown)
            {
                view.SetPreferredMinSize(minSize);
                if (useMinSize)
                {
                    _ = view.TryResizeView(minSize);
                }
            }

            return shown;
        }

        /// <summary>
        /// Creates a new <see cref="ApplicationView"/> which hosts a frame
        /// that navigates to <typeparamref name="TPage"/> with the provided
        /// parameter.
        /// </summary>
        public static Task<ApplicationView> CreateViewAsync<TPage>(object parameter = null)
            where TPage : Page
        {
            CoreApplicationView window = CoreApplication.CreateNewView();
            TaskCompletionSource<ApplicationView> tcs = new();

            _ = window.DispatcherQueue.TryEnqueue(() =>
            {
                Frame frame = new();
                _ = frame.Navigate(typeof(TPage), parameter);

                Window curr = Window.Current;
                curr.Content = frame;
                curr.Activate();

                tcs.SetResult(ApplicationView.GetForCurrentView());
            });

            return tcs.Task;
        }
    }
}
