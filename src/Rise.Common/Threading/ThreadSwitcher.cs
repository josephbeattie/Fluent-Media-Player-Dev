using System.ComponentModel;
using Windows.System.Threading;
using Windows.UI.Core;

namespace Rise.Common.Threading
{
    /// <summary>
    /// A helper class that provides awaiters for multiple WinRT
    /// objects.
    /// </summary>
    public static class ThreadSwitcher
    {
        public static CoreDispatcherAwaiter ResumeForegroundAsync(CoreDispatcher dispatcher)
        {
            return new(dispatcher);
        }

        /// <summary>
        /// Configures a dispatcher awaiter with the provided priority.
        /// </summary>
        /// <returns>The configured awaiter.</returns>
        public static ConfiguredCoreDispatcherAwaiter ConfigureAwait(this CoreDispatcher dispatcher, CoreDispatcherPriority priority)
        {
            return new(dispatcher, priority);
        }

        public static ThreadPoolAwaiter ResumeBackgroundAsync()
        {
            return new();
        }

        /// <summary>
        /// Configures the awaiter with the provided priority.
        /// </summary>
        /// <returns>The configured awaiter.</returns>
        public static ConfiguredThreadPoolAwaiter ResumeBackgroundAsync(WorkItemPriority priority)
        {
            return new(priority, WorkItemOptions.None);
        }

        /// <summary>
        /// Configures the awaiter with the provided priority and options.
        /// </summary>
        /// <returns>The configured awaiter.</returns>
        public static ConfiguredThreadPoolAwaiter ResumeBackgroundAsync(WorkItemPriority priority, WorkItemOptions options)
        {
            return new(priority, options);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static CoreDispatcherAwaiter GetAwaiter(this CoreDispatcher dispatcher)
        {
            return new(dispatcher);
        }
    }
}
