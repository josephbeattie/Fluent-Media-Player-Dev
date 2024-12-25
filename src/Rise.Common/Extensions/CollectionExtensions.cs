using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rise.Common.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Move the provided item to the provided index.
        /// </summary>
        public static void Move<T>(this IList<T> list, T item, int index)
        {
            _ = list.Remove(item);
            list.Insert(index, item);
        }

        /// <summary>
        /// Move item at oldIndex to newIndex.
        /// </summary>
        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            T item = list[oldIndex];

            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }

        /// <summary>
        /// Moves a set of items to the end of the list.
        /// </summary>
        /// <param name="items">Items to move.</param>
        public static void MoveItemsToEnd<T>(this IList<T> list, T[] items)
        {
            foreach (var item in items)
            {
                int moveFrom = list.IndexOf(item);
                list.Move(moveFrom, list.Count - 1);
            }
        }

        /// <summary>
        /// Moves the items between <paramref name="startIndex"/> and
        /// <paramref name="endIndex"/> (inclusive) to the end of the list.
        /// </summary>
        public static void MoveRangeToEnd<T>(this IList<T> list,
            int startIndex, int endIndex)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (endIndex > list.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(endIndex));

            for (int i = startIndex; i <= endIndex; i++)
            {
                list.Move(startIndex, list.Count - 1);
            }
        }

        public static bool MoveItemInCollection<T>(this Collection<T> collection, T item, int newIndex)
        {
            if (collection?.Remove(item) ?? false)
            {
                collection.Insert(newIndex, item);
                return true;
            }

            return false;
        }

        public static bool AddIfNotExists<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Searches the entire sorted <see cref="IList{T}"/> for an element using
        /// the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="value">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing
        /// elements. -or- null to use the default comparer <see cref="Comparer{T}.Default"/>.</param>
        /// <returns>The zero-based index of item in the sorted <see cref="IList{T}"/>,
        /// if item is found; otherwise, a negative number that is the bitwise complement
        /// of the index of the next element that is larger than item or, if there is no
        /// larger element, the bitwise complement of <see cref="ICollection{T}.Count"/>.</returns>
        /// <exception cref="ArgumentNullException">The provided list was null.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="comparer"/> is null, and the
        /// default comparer <see cref="Comparer{T}.Default"/> cannot find an implementation of the
        /// <see cref="IComparable{T}"/> generic interface or the <see cref="IComparable"/> interface
        /// for type <typeparamref name="T"/>.</exception>
        public static int BinarySearch<T>(this IList<T> list, T value, IComparer<T> comparer = null)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            comparer ??= Comparer<T>.Default;

            int lower = 0;
            int upper = list.Count - 1;

            while (lower <= upper)
            {
                int middle = lower + ((upper - lower) >> 1);
                int comparisonResult = comparer.Compare(list[middle], value);

                if (comparisonResult == 0)
                    return middle;
                else if (comparisonResult < 0)
                    lower = middle + 1;
                else
                    upper = middle - 1;
            }

            return ~lower;
        }
    }
}
