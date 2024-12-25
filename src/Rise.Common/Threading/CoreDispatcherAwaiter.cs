using System;
using System.Runtime.CompilerServices;
using Windows.UI.Core;

namespace Rise.Common.Threading
{
    /// <summary>
    /// A custom awaiter for <see cref="CoreDispatcher"/> objects,
    /// dispatching its continuation with normal priority.
    /// </summary>
    public struct CoreDispatcherAwaiter : INotifyCompletion
    {
        private readonly CoreDispatcher dispatcher;

        internal CoreDispatcherAwaiter(CoreDispatcher dispatcher)
            => this.dispatcher = dispatcher;

        public CoreDispatcherAwaiter GetAwaiter() => this;
        public bool IsCompleted => dispatcher.HasThreadAccess;

        public void GetResult() { }
        public void OnCompleted(Action continuation)
            => _ = dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => continuation());
    }

    /// <summary>
    /// A custom awaiter for <see cref="CoreDispatcher"/> objects,
    /// dispatching its continuation with the provided priority.
    /// </summary>
    public struct ConfiguredCoreDispatcherAwaiter : INotifyCompletion
    {
        private readonly CoreDispatcher dispatcher;
        private readonly CoreDispatcherPriority priority;

        internal ConfiguredCoreDispatcherAwaiter(CoreDispatcher dispatcher, CoreDispatcherPriority priority)
        {
            this.dispatcher = dispatcher;
            this.priority = priority;
        }

        public ConfiguredCoreDispatcherAwaiter GetAwaiter() => this;
        public bool IsCompleted => dispatcher.HasThreadAccess;

        public void GetResult() { }
        public void OnCompleted(Action continuation)
            => _ = dispatcher.RunAsync(priority, () => continuation());
    }
}
