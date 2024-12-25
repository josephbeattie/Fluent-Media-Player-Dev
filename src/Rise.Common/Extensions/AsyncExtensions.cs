using System;
using System.Threading;
using Windows.Foundation;

namespace Rise.Common.Extensions
{
    /// <summary>
    /// This class contains helper methods that aid in async
    /// work with WinRT's IAsync* types.
    /// </summary>
    public static class AsyncExtensions
    {
        /// <summary>
        /// Runs the provided action synchronously.
        /// </summary>
        public static void Get(this IAsyncAction ac)
        {
            if (ac.Status == AsyncStatus.Started)
            {
                using (var evt = new ManualResetEventSlim(false))
                {
                    ac.Completed = (s, e) => evt.Set();
                    evt.Wait();
                }
            }
            ThrowIfCanceled(ac.Status);
            ac.GetResults();
        }

        /// <summary>
        /// Runs the provided action synchronously, running the provided
        /// progress handler when necessary.
        /// </summary>
        public static void Get<TProgress>(this IAsyncActionWithProgress<TProgress> ac,
            AsyncActionProgressHandler<TProgress> progressHandler = null)
        {
            if (ac.Status == AsyncStatus.Started)
            {
                using (var evt = new ManualResetEventSlim(false))
                {
                    ac.Completed = (s, e) => evt.Set();
                    ac.Progress = progressHandler;
                    evt.Wait();
                }
            }
            ThrowIfCanceled(ac.Status);
            ac.GetResults();
        }

        /// <summary>
        /// Runs the provided operation synchronously and returns the
        /// result.
        /// </summary>
        public static TResult Get<TResult>(this IAsyncOperation<TResult> op)
        {
            if (op.Status == AsyncStatus.Started)
            {
                using (var evt = new ManualResetEventSlim(false))
                {
                    op.Completed = (s, e) => evt.Set();
                    evt.Wait();
                }
            }
            ThrowIfCanceled(op.Status);
            return op.GetResults();
        }

        /// <summary>
        /// Runs the provided operation synchronously and returns the
        /// result, running the provided progress handler when necessary.
        /// </summary>
        public static TResult Get<TResult, TProgress>(this IAsyncOperationWithProgress<TResult, TProgress> op,
            AsyncOperationProgressHandler<TResult, TProgress> progressHandler = null)
        {
            if (op.Status == AsyncStatus.Started)
            {
                using (var evt = new ManualResetEventSlim(false))
                {
                    op.Completed = (s, e) => evt.Set();
                    op.Progress = progressHandler;
                    evt.Wait();
                }
            }
            ThrowIfCanceled(op.Status);
            return op.GetResults();
        }

        private static void ThrowIfCanceled(AsyncStatus status)
        {
            if (status == AsyncStatus.Canceled)
                throw new OperationCanceledException();
        }
    }
}
