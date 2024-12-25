using System.Collections;
using System.Collections.Generic;
using Windows.Foundation.Collections;

namespace Rise.Common.Helpers
{
    /// <summary>
    /// A class that implements <see cref="IObservableVector{T}"/>
    /// for .NET usage. Use this if you're writing a WinRT component
    /// in C# and need an observable vector to expose.
    /// </summary>
    public sealed class ObservableVector<T> : IObservableVector<T>
    {
        private readonly List<T> _base;
        public T this[int index]
        {
            get => _base[index];
            set
            {
                _base[index] = value;
                VectorChanged?.Invoke(this, new VectorChangedEventArgs(CollectionChange.ItemChanged, (uint)index));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableVector{T}"/> class that
        /// is empty and has the default initial capacity.
        /// </summary>
        public ObservableVector()
        {
            _base = new();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableVector{T}"/> class that
        /// contains elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public ObservableVector(IEnumerable<T> collection)
        {
            _base = new(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableVector{T}"/> class that
        /// is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        public ObservableVector(int capacity)
        {
            _base = new(capacity);
        }

        public int Count => _base.Count;
        public bool IsReadOnly { get; private set; }
            = false;

        public event VectorChangedEventHandler<T> VectorChanged;
        public int IndexOf(T item)
            => _base.IndexOf(item);

        public void Add(T item)
        {
            _base.Add(item);
            VectorChanged?.Invoke(this, new VectorChangedEventArgs(CollectionChange.ItemInserted, (uint)_base.Count - 1));
        }

        public void Insert(int index, T item)
        {
            _base.Insert(index, item);
            VectorChanged?.Invoke(this, new VectorChangedEventArgs(CollectionChange.ItemInserted, (uint)index));
        }

        public bool Remove(T item)
        {
            int index = _base.IndexOf(item);
            if (index != -1)
            {
                _base.RemoveAt(index);
                VectorChanged?.Invoke(this, new VectorChangedEventArgs(CollectionChange.ItemRemoved, (uint)index));
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            _base.RemoveAt(index);
            VectorChanged?.Invoke(this, new VectorChangedEventArgs(CollectionChange.ItemRemoved, (uint)index));
        }

        public void Clear()
        {
            _base.Clear();
            VectorChanged?.Invoke(this, new VectorChangedEventArgs(CollectionChange.Reset, 0));
        }

        public bool Contains(T item)
            => _base.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
            => _base.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _base.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _base.GetEnumerator();
    }

    /// <summary>
    /// A class that implements <see cref="IVectorChangedEventArgs"/>
    /// for .NET usage. Use this if you're writing a WinRT component
    /// in C# and need an observable vector to expose.
    /// </summary>
    public sealed class VectorChangedEventArgs : IVectorChangedEventArgs
    {
        public CollectionChange CollectionChange { get; }
        public uint Index { get; }

        public VectorChangedEventArgs(CollectionChange change, uint index)
        {
            CollectionChange = change;
            Index = index;
        }
    }
}
