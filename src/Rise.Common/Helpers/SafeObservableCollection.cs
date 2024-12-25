using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace Rise.Common.Helpers
{
    /// <summary>
    /// An observable collection class with thread safety and ranges in mind.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public partial class SafeObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="SafeObservableCollection{T}"/> class.
        /// </summary>
        public SafeObservableCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="SafeObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection from which the
        /// elements are copied.</param>
        /// <exception cref="ArgumentNullException">The collection
        /// parameter cannot be null.</exception>
        public SafeObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Adds the elements from the specified collection
        /// to the end of this collection.
        /// </summary>
        public void AddRange(IEnumerable<T> items)
        {
            CheckReentrancy();

            int startIndex = Count;
            foreach (T item in items)
            {
                this.Items.Add(item);
            }

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));

            OnCollectionChanged(new NotifyCollectionChangedEventArgs
                (NotifyCollectionChangedAction.Add, items, startIndex));
        }

        /// <summary>
        /// Removes the elements in the specified
        /// collection from this collection.
        /// </summary>
        public void RemoveRange(IEnumerable<T> items)
        {
            CheckReentrancy();

            bool raiseEvents = false;
            foreach (T item in items)
            {
                if (this.Items.Remove(item))
                {
                    raiseEvents = true;
                }
            }

            if (raiseEvents)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));

                OnCollectionChanged(new NotifyCollectionChangedEventArgs
                    (NotifyCollectionChangedAction.Reset));
            }
        }
    }

    public partial class SafeObservableCollection<T>
    {
        // Property change events
        private readonly Dictionary<NotifyCollectionChangedEventHandler, SynchronizationContext> CollectionChangedEvents =
            new Dictionary<NotifyCollectionChangedEventHandler, SynchronizationContext>();

        private readonly Dictionary<PropertyChangedEventHandler, SynchronizationContext> PropertyChangedEvents =
            new Dictionary<PropertyChangedEventHandler, SynchronizationContext>();

        public override event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                CollectionChangedEvents.Add(value, SynchronizationContext.Current);
            }
            remove
            {
                CollectionChangedEvents.Remove(value);
            }
        }

        protected override event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                PropertyChangedEvents.Add(value, SynchronizationContext.Current);
            }
            remove
            {
                PropertyChangedEvents.Remove(value);
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            using (BlockReentrancy())
            {
                foreach (KeyValuePair<NotifyCollectionChangedEventHandler, SynchronizationContext> @event in CollectionChangedEvents)
                {
                    if (@event.Value == null)
                    {
                        @event.Key.Invoke(this, e);
                    }
                    else
                    {
                        @event.Value.Post(s => @event.Key.Invoke(s, e), this);
                    }
                }
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            foreach (KeyValuePair<PropertyChangedEventHandler, SynchronizationContext> @event in PropertyChangedEvents)
            {
                if (@event.Value == null)
                {
                    @event.Key.Invoke(this, e);
                }
                else
                {
                    @event.Value.Post(s => @event.Key.Invoke(s, e), this);
                }
            }
        }
    }
}
