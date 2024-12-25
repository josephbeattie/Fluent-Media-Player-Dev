using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI.Xaml.Controls;
using Rise.App.ViewModels;
using Rise.App.Views;
using Rise.Common.Constants;
using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using Rise.Data.Messages;
using Rise.Data.Navigation;
using Rise.Data.ViewModels;
using Rise.Effects;
using Rise.NewRepository;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private static Timer IndexingTimer;

        // No lazy init (used very early on, so lazy init is not needed)
        public static MainViewModel MViewModel { get; } = new();
        public static SettingsViewModel SViewModel { get; } = new();
        public static NavigationDataSource NavDataSource { get; } = new();

        // Lazy init
        private static readonly Lazy<MediaPlaybackViewModel> _mpViewModel
            = new(OnMPViewModelRequested);
        public static MediaPlaybackViewModel MPViewModel => _mpViewModel.Value;

        private static readonly Lazy<LastFMViewModel> _lmViewModel
            = new(OnLFMRequested);
        public static LastFMViewModel LMViewModel => _lmViewModel.Value;

        private static readonly Lazy<StorageLibrary> _musicLibrary
            = new(OnStorageLibraryRequested(KnownLibraryId.Music));
        public static StorageLibrary MusicLibrary => _musicLibrary.Value;

        private static readonly Lazy<StorageLibrary> _videoLibrary
            = new(OnStorageLibraryRequested(KnownLibraryId.Videos));
        public static StorageLibrary VideoLibrary => _videoLibrary.Value;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            int theme = SViewModel.Theme;
            if (theme == 0)
            {
                RequestedTheme = ApplicationTheme.Light;
            }
            else if (theme == 1)
            {
                RequestedTheme = ApplicationTheme.Dark;
            }

            // Reset the glaze color before startup if necessary
            if (SViewModel.SelectedGlaze == GlazeTypes.MediaThumbnail)
            {
                SViewModel.GlazeColors = Colors.Transparent;
            }

            InitializeComponent();

            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;

            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            CoreApplication.UnhandledErrorDetected += OnUnhandledErrorDetected;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            await ActivateAsync(e.PreviousExecutionState, e.PrelaunchActivated);
        }

        protected override async void OnActivated(IActivatedEventArgs e)
        {
            switch (e.Kind)
            {
                case ActivationKind.StartupTask:
                    {
                        await ActivateAsync(e.PreviousExecutionState, false);
                        break;
                    }

                case ActivationKind.ToastNotification:
                    {
                        if (e is ToastNotificationActivatedEventArgs toastActivationArgs)
                        {
                            await ActivateAsync(e.PreviousExecutionState, false);
                            QueryString args = QueryString.Parse(toastActivationArgs.Argument);

                            // If the exception name equals to null,
                            // then the toast likely isn't popping up
                            // as a result of an app crash.
                            if (args["exceptionName"] != null)
                            {
                                string text = $"The exception {args["exceptionName"]} happened last time the app was launched.\n\nStack trace:\n{args["message"]}\n{args["stackTrace"]}\nSource: {args["source"]}\nHResult: {args["hresult"]}";
                                _ = await CrashDetailsPage.TryShowAsync(text);
                            }
                        }
                        break;
                    }
            }
        }

        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            await ActivateAsync(args.PreviousExecutionState, false);

            System.Collections.Generic.IEnumerable<StorageFile> files = args.Files.OfType<StorageFile>();
            await MPViewModel.PlayFilesAsync(files);
        }

        private async void OnDrop(object sender, DragEventArgs e)
        {
            if (!e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                return;
            }

            System.Collections.Generic.IEnumerable<StorageFile> files = (await e.DataView.GetStorageItemsAsync()).OfType<StorageFile>();
            await MPViewModel.PlayFilesAsync(files);
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Link;

            DragUIOverride dragOverride = e.DragUIOverride;
            if (dragOverride != null)
            {
                dragOverride.Caption = ResourceHelper.GetString("PlayMedia");
                dragOverride.IsContentVisible = true;
            }
        }

        /// <summary>
        /// Activates the app's window and puts content in there
        /// if necessary.
        /// </summary>
        /// <param name="previousState">Previous app execution state.</param>
        /// <param name="prelaunched">Whether the app was prelaunched.</param>
        private async Task ActivateAsync(ApplicationExecutionState previousState, bool prelaunched)
        {
            Window window = Window.Current;
            if (window.Content is not Frame rootFrame)
            {
                rootFrame = new Frame();
                window.Content = rootFrame;

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.AllowDrop = true;
                rootFrame.DragOver += OnDragOver;
                rootFrame.Drop += OnDrop;

                // The backdrop can be applied to the frame directly
                // https://learn.microsoft.com/en-us/windows/apps/design/style/mica#use-mica-with-winui-2-for-uwp
                BackdropMaterial.SetApplyToRootOrPageBackground(rootFrame, true);
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // Restore the saved session state only when appropriate
                if ((previousState == ApplicationExecutionState.Terminated) ||
                    (previousState == ApplicationExecutionState.ClosedByUser &&
                    SViewModel.PickUp))
                {
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (Exception e)
                    {
                        e.WriteToOutput();
                    }
                }

                await Repository.InitializeDatabaseAsync();
            }

            if (!prelaunched)
            {
                CoreApplication.EnablePrelaunch(true);
                if (rootFrame.Content == null)
                {
                    _ = !SViewModel.SetupCompleted
                        ? rootFrame.Navigate(typeof(SetupPage))
                        : rootFrame.Navigate(typeof(MainPage));
                }

                window.Activate();
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            try
            {
                await SuspensionManager.SaveAsync();
            }
            catch (Exception ex)
            {
                ex.WriteToOutput();
            }
            finally
            {
                deferral.Complete();
            }
        }
    }

    // Data source/ViewModel initialization
    public sealed partial class App
    {
        private static LastFMViewModel OnLFMRequested()
        {
            LastFMViewModel lfm = new(LastFM.Key, LastFM.Secret);
            _ = lfm.TryLoadCredentials(LastFM.VaultResource);
            return lfm;
        }

        private static MediaPlaybackViewModel OnMPViewModelRequested()
        {
            MediaPlaybackViewModel mpvm = new();

            if (!EqualizerEffect.Initialized)
            {
                EqualizerEffect eq = EqualizerEffect.Current;
                eq.InitializeBands(SViewModel.EqualizerGain);
                eq.IsEnabled = SViewModel.EqualizerEnabled;
            }

            mpvm.AddEffect(new(typeof(EqualizerEffect), false, true, null));
            return mpvm;
        }

        private static StorageLibrary OnStorageLibraryRequested(KnownLibraryId id)
        {
            StorageLibrary library = StorageLibrary.GetLibraryAsync(id).Get();
            library.ChangeTracker.Enable();
            return library;
        }
    }

    // Indexing
    public sealed partial class App
    {
        // Change tracking
        public static async Task InitializeChangeTrackingAsync()
        {
            if (SViewModel.IndexingFileTrackingEnabled)
            {
                _ = await MusicLibrary.TrackBackgroundAsync($"{nameof(MusicLibrary)} background tracker");
                BackgroundTaskRegistrationStatus result = await VideoLibrary.TrackBackgroundAsync($"{nameof(VideoLibrary)} background tracker");

                // If the trackers were registered successfully, we also have to
                // track definition changes
                if (result is BackgroundTaskRegistrationStatus.Successful or
                    BackgroundTaskRegistrationStatus.AlreadyExists)
                {
                    MusicLibrary.DefinitionChanged += OnLibraryDefinitionChanged;
                    VideoLibrary.DefinitionChanged += OnLibraryDefinitionChanged;
                    return;
                }
            }

            // If file tracking is off, or the background tasks can't be
            // registered, use the indexing timer
            RestartIndexingTimer();
        }

        private static async void OnLibraryDefinitionChanged(StorageLibrary sender, object args)
        {
            await MViewModel.StartFullCrawlAsync();
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            Windows.ApplicationModel.Background.IBackgroundTaskInstance instance = args.TaskInstance;
            Windows.ApplicationModel.Background.BackgroundTaskDeferral deferral = instance.GetDeferral();

            await Repository.InitializeDatabaseAsync();

            // Check whether the task was triggered for the music or the video library
            string name = instance.Task.Name;
            if (name.Contains(nameof(MusicLibrary)))
            {
                await MViewModel.HandleLibraryChangesAsync(ChangedLibraryType.Music);
            }
            else if (name.Contains(nameof(VideoLibrary)))
            {
                await MViewModel.HandleLibraryChangesAsync(ChangedLibraryType.Videos);
            }

            deferral?.Complete();
        }

        // Indexing timer
        public static void RestartIndexingTimer()
        {
            if (IndexingTimer != null && IndexingTimer.Enabled)
            {
                IndexingTimer.Stop();
            }

            if (!SViewModel.IndexingTimerEnabled)
            {
                return;
            }

            TimeSpan span = TimeSpan.FromMinutes(SViewModel.IndexingTimerInterval);
            IndexingTimer = new(span.TotalMilliseconds)
            {
                AutoReset = true
            };

            IndexingTimer.Elapsed += IndexingTimer_Elapsed;
            IndexingTimer.Start();
        }

        private static async void IndexingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await MViewModel.HandleLibraryChangesAsync(ChangedLibraryType.Both, true);

            await Repository.UpsertQueuedAsync();
            await Repository.DeleteQueuedAsync();
        }
    }

    // Error handling
    public sealed partial class App
    {
        /// <summary>
        /// Shows a toast with the provided exception data.
        /// </summary>
        private void ShowExceptionToast(Exception e)
        {
            string notifTitle = ResourceHelper.GetString("ErrorOcurred");
            ToastContent content = new ToastContentBuilder()
                .AddToastActivationInfo(new QueryString()
                {
                     { "stackTrace", e.StackTrace },
                     { "message", e.Message },
                     { "exceptionName", e.GetType().ToString() },
                     { "source", e.Source },
                     { "hresult", $"{e.HResult}" }
                }.ToString(), ToastActivationType.Foreground)
                .AddText(notifTitle)
                .AddText(ResourceHelper.GetString("CrashStackTrace"))
                .GetToastContent();

            ToastNotification notification = new(content.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(notification);

            StringBuilder builder = new();

            _ = builder.Append(ResourceHelper.GetString("CrashDetails"));
            _ = builder.Append("\n\n");
            _ = builder.AppendLine("-----");
            _ = builder.Append("Exception type: ");
            _ = builder.AppendLine(e.GetType().ToString());

            _ = builder.Append("HRESULT: ");
            _ = builder.AppendLine(e.HResult.ToString());
            _ = builder.Append("Source: ");
            _ = builder.AppendLine(e.Source);

            _ = builder.AppendLine();

            _ = builder.AppendLine("Message:");
            _ = builder.AppendLine(e.Message);
            _ = builder.AppendLine();
            _ = builder.AppendLine("Stack trace:");
            _ = builder.AppendLine(e.StackTrace);
            _ = builder.AppendLine("-----");

            BasicNotification notif = new(notifTitle, builder.ToString(), "\uE8BB");

            MViewModel.NBackend.Items.Add(notif);
            MViewModel.NBackend.Save();
        }

        private void OnUnhandledErrorDetected(object sender, UnhandledErrorDetectedEventArgs e)
        {
            // We can't recover in this case, so logging and throwing is
            // all we can do
            if (!e.UnhandledError.Handled)
            {
                try
                {
                    e.UnhandledError.Propagate();
                }
                catch (Exception ex)
                {
                    ex.WriteToOutput();
                    ShowExceptionToast(ex);

                    throw;
                }
            }
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Exception.WriteToOutput();
            ShowExceptionToast(e.Exception);
        }

        private void OnCurrentDomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            (e.ExceptionObject as Exception).WriteToOutput();
            ShowExceptionToast(e.ExceptionObject as Exception);
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.Exception.WriteToOutput();
            ShowExceptionToast(e.Exception);
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails.
        /// </summary>
        /// <param name="sender">The Frame which failed navigation.</param>
        /// <param name="e">Details about the navigation failure.</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
