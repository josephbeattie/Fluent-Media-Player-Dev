using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Windows.System.Threading;
using ThreadPool = Windows.System.Threading.ThreadPool;

namespace Rise.Common.Threading
{
    /// <summary>
    /// A custom awaiter for the Windows thread pool, adding its continuation
    /// as a work item with normal priority.
    /// </summary>
    public struct ThreadPoolAwaiter : INotifyCompletion
    {
        public ThreadPoolAwaiter GetAwaiter() => this;
        public bool IsCompleted => SynchronizationContext.Current == null;

        public void GetResult() { }
        public void OnCompleted(Action continuation)
            => _ = ThreadPool.RunAsync(_ => continuation());
    }

    /// <summary>
    /// A custom awaiter for the Windows thread pool, adding its continuation
    /// as a work item with the provided priority and options.
    /// </summary>
    public struct ConfiguredThreadPoolAwaiter : INotifyCompletion
    {
        private readonly WorkItemPriority priority;
        private readonly WorkItemOptions options;

        internal ConfiguredThreadPoolAwaiter(WorkItemPriority priority, WorkItemOptions options)
        {
            this.priority = priority;
            this.options = options;
        }

        public ConfiguredThreadPoolAwaiter GetAwaiter() => this;
        public bool IsCompleted => SynchronizationContext.Current == null;

        public void GetResult() { }
        public void OnCompleted(Action continuation)
            => _ = ThreadPool.RunAsync(_ => continuation(), priority, options);
    }
}
