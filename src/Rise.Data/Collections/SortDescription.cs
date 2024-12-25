using System;
using System.Collections;
using System.Collections.Generic;

namespace Rise.Data.Collections;

public sealed class SortDescription : IComparer, IComparer<object>
{
    private readonly Func<object, object> _valueDelegate;
    /// <summary>
    /// Gets the delegate that returns the value of the property
    /// to sort on.
    /// </summary>
    public Func<object, object> ValueDelegate => _valueDelegate;

    private readonly SortDirection _sortDirection;
    /// <summary>
    /// Gets the way sorting should be handled.
    /// </summary>
    public SortDirection SortDirection => _sortDirection;

    private readonly IComparer _comparer;
    /// <summary>
    /// The comparer to use for sorting.
    /// </summary>
    public IComparer Comparer => _comparer;

    /// <summary>
    /// Creates a new sort description.
    /// </summary>
    /// <param name="direction">The way sorting should be handled.</param>
    public SortDescription(SortDirection direction)
        : this(direction, (o) => o, null) { }

    /// <summary>
    /// Creates a new sort description.
    /// </summary>
    /// <param name="direction">The way sorting should be handled.</param>
    /// <param name="valueDelegate">A delegate that accepts an object as a
    /// parameter and returns the value to sort on.</param>
    public SortDescription(SortDirection direction, Func<object, object> valueDelegate)
        : this(direction, valueDelegate, null) { }

    /// <summary>
    /// Creates a new sort description.
    /// </summary>
    /// <param name="direction">The way sorting should be handled.</param>
    /// <param name="valueDelegate">A delegate that accepts an object as a
    /// parameter and returns the value to sort on.</param>
    /// <param name="comparer">Comparer to use. If null, a default comparer
    /// will be used.</param>
    public SortDescription(SortDirection direction, Func<object, object> valueDelegate, IComparer comparer)
    {
        _sortDirection = direction;
        _valueDelegate = valueDelegate;
        _comparer = comparer ?? ObjectComparer.Default;
    }

    private SortDescription(SortDescription desc)
    {
        _sortDirection = desc._sortDirection == SortDirection.Ascending ?
            SortDirection.Descending : SortDirection.Ascending;

        _valueDelegate = desc._valueDelegate;
        _comparer = desc._comparer;
    }

    /// <summary>
    /// Creates a sort description with the direction inverted.
    /// </summary>
    public SortDescription Invert()
        => new(this);

    public int Compare(object x, object y)
    {
        int result = _comparer.Compare(_valueDelegate(x), _valueDelegate(y));
        return _sortDirection == SortDirection.Ascending ? +result : -result;
    }

    private sealed class ObjectComparer : IComparer
    {
        public static readonly ObjectComparer Default = new();
        private ObjectComparer() { }

        public int Compare(object x, object y)
        {
            var cx = x as IComparable;
            var cy = y as IComparable;

            return cx == cy ? 0 : cx == null ? -1 : cy == null ? +1 : cx.CompareTo(cy);
        }
    }
}
