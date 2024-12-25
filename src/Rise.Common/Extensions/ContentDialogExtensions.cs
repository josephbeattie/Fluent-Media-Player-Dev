using Rise.Common.Enums;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Rise.Common.Extensions
{
    /// <summary>
    /// Some helpers to better manage ContentDialogs.
    /// </summary>
    public static class ContentDialogExtensions
    {
        public static ContentDialog ActiveDialog;
        private static TaskCompletionSource<bool> _dialogAwaiter = new();

        /// <summary>
        /// Opens a <see cref="ContentDialog"/> with the specified options.
        /// </summary>
        /// <param name="dialog">Dialog to open.</param>
        /// <param name="option">What to do with the previously open dialog.</param>
        public static async Task<ContentDialogResult> ShowAsync(this ContentDialog dialog, ExistingDialogOptions option)
            => await Show(dialog, option);

        // Huge thanks to Notepads:
        // https://github.com/JasonStein/Notepads/blob/f127d170c16cbf0831c2cddb480a3ea05e202930/src/Notepads/Utilities/DialogManager.cs
        // Relevant thread: https://github.com/microsoft/microsoft-ui-xaml/issues/1679
        private static async Task<ContentDialogResult> Show(this ContentDialog dialog, ExistingDialogOptions option)
        {
            TaskCompletionSource<bool> currentAwaiter = _dialogAwaiter;
            TaskCompletionSource<bool> nextAwaiter = new();

            _dialogAwaiter = nextAwaiter;

            if (ActiveDialog != null)
            {
                switch (option)
                {
                    case ExistingDialogOptions.CloseExisting:
                        ActiveDialog.Hide();
                        break;

                    case ExistingDialogOptions.Enqueue:
                        await currentAwaiter.Task;
                        break;
                }
            }

            ActiveDialog = dialog;
            ContentDialogResult result = await ActiveDialog.ShowAsync();
            nextAwaiter.SetResult(true);

            return result;
        }
    }
}
