using Rise.Common.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;

namespace Rise.Data.Collections;

/// <summary>
/// Represents a collection of items within a group.
/// </summary>
public sealed class CollectionViewGroup : ICollectionViewGroup, IGrouping<object, object>
{
    public object Key => Group;

    public object Group { get; }
    public IObservableVector<object> GroupItems { get; }

    public CollectionViewGroup(object group)
    {
        Group = group;
        GroupItems = new ObservableVector<object>();
    }

    public CollectionViewGroup(IGrouping<object, object> group)
    {
        Group = group.Key;
        GroupItems = new ObservableVector<object>(group);
    }

    public CollectionViewGroup(object group, IEnumerable<object> items)
    {
        Group = group;
        GroupItems = new ObservableVector<object>(items);
    }

    public IEnumerator<object> GetEnumerator() => GroupItems.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GroupItems.GetEnumerator();
}
