using Rise.Common.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;

namespace Rise.Data.Collections;

/// <summary>
/// An implementation for <see cref="ICollectionView"/> with grouping
/// and incremental loading support.
/// </summary>
public sealed partial class GroupedCollectionView : ICollectionView, ISupportIncrementalLoading,
    INotifyPropertyChanged, IComparer<object>, IDisposable
{
    private readonly List<object> _view = new();

    private SortDescription _groupDescription;
    /// <summary>
    /// A sort description used for item grouping.
    /// </summary>
    public SortDescription GroupDescription
    {
        get => _groupDescription;
        set
        {
            if (_groupDescription != value)
            {
                _groupDescription = value;

                if (_groupDescription != null)
                    _collectionGroupComparer = new CollectionGroupComparer(value);

                if (_deferCounter == 0)
                    OnSortDescriptionsChanged(CurrentItem);

                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Whether the view is currently grouped.
    /// </summary>
    public bool IsGrouped => _groupDescription != null;

    private IComparer<object> _collectionGroupComparer;

    private readonly ObservableVector<object> _collectionGroups = new();
    public IObservableVector<object> CollectionGroups => _collectionGroups;

    private readonly ObservableCollection<SortDescription> _sortDescriptions;
    /// <summary>
    /// A list of objects that define sorting rules for visible items.
    /// </summary>
    public ObservableCollection<SortDescription> SortDescriptions => _sortDescriptions;

    private Predicate<object> _filter;
    /// <summary>
    /// A predicate to filter objects from the collection.
    /// </summary>
    public Predicate<object> Filter
    {
        get => _filter;
        set
        {
            if (_filter != value)
            {
                _filter = value;
                if (_deferCounter == 0)
                    OnFilterChanged();

                OnPropertyChanged();
            }
        }
    }

    private IList _source;
    /// <summary>
    /// Gets or sets the source for this collection view.
    /// </summary>
    public IList Source
    {
        get => _source;
        set
        {
            if (_source == value)
                return;

            if (_source is INotifyCollectionChanged oldNcc)
                oldNcc.CollectionChanged -= OnSourceCollectionChanged;

            _source = value;
            if (_source is INotifyCollectionChanged ncc)
                ncc.CollectionChanged += OnSourceCollectionChanged;

            if (_deferCounter == 0)
                OnSourceChanged();

            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="GroupedCollectionView"/>
    /// with an empty list.
    /// </summary>
    public GroupedCollectionView()
    {
        _sortDescriptions = new();
        _sortDescriptions.CollectionChanged += OnSortDescriptionsChanged;

        _source = new List<object>();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="GroupedCollectionView"/>
    /// with the provided data source.
    /// </summary>
    public GroupedCollectionView(IList source)
    {
        _sortDescriptions = new();
        _sortDescriptions.CollectionChanged += OnSortDescriptionsChanged;

        Source = source;
    }

    /// <summary>
    /// Creates a <see cref="GroupedCollectionView"/> that gets immediately
    /// deferred upon creation.
    /// </summary>
    /// <returns>A tuple with the collection and the deferral.</returns>
    public static (GroupedCollectionView, Deferral) CreateDeferred()
    {
        var collection = new GroupedCollectionView();
        var deferral = collection.DeferRefresh();

        return (collection, deferral);
    }

    private void OnSortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (_deferCounter != 0)
            return;

        OnSortDescriptionsChanged(CurrentItem);
    }

    private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (_deferCounter != 0)
            return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems?.Count == 1)
                    OnItemAdded(e.NewStartingIndex, e.NewItems[0]);
                else
                    OnSourceChanged();
                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems?.Count == 1)
                    OnItemRemoved(e.OldStartingIndex, e.OldItems[0]);
                else
                    OnSourceChanged();
                break;

            case NotifyCollectionChangedAction.Replace:
                if (e.OldItems?.Count == 1)
                {
                    OnItemRemoved(e.OldStartingIndex, e.OldItems[0]);
                    OnItemAdded(e.OldStartingIndex, e.NewItems[0]);
                }
                else
                {
                    OnSourceChanged();
                }
                break;

            case NotifyCollectionChangedAction.Move:
                if (e.OldItems?.Count == 1)
                {
                    OnItemRemoved(e.OldStartingIndex, e.OldItems[0]);
                    OnItemAdded(e.NewStartingIndex, e.OldItems[0]);

                    MoveCurrentToIndex(e.NewStartingIndex);
                }
                else
                {
                    OnSourceChanged();
                }
                break;

            case NotifyCollectionChangedAction.Reset:
                OnSourceChanged();
                break;
        }
    }

    private void OnSourceChanged()
    {
        _incrementalSource = _source as ISupportIncrementalLoading;

        var current = CurrentItem;
        _view.Clear();

        foreach (var item in _source)
        {
            if (_filter != null && !_filter(item))
                continue;

            _view.Add(item);
        }

        OnSortDescriptionsChanged(current);
    }

    private void OnSortDescriptionsChanged(object currentItem)
    {
        _view.Sort(this);

        _collectionGroups.Clear();
        if (_groupDescription != null)
        {
            // The view is already sorted, so we just have to add
            // these groups to the collection
            var grouped = _view.GroupBy(_groupDescription.ValueDelegate);
            foreach (var group in grouped)
            {
                var cvw = new CollectionViewGroup(group);
                _collectionGroups.Add(cvw);
            }
        }

        OnVectorChanged(CollectionChange.Reset, 0);
        OnPropertyChanged(nameof(IsGrouped));

        _ = MoveCurrentTo(currentItem);
    }

    private void OnFilterChanged()
    {
        if (_filter != null)
        {
            for (int index = 0; index < _view.Count; index++)
            {
                var item = _view.ElementAt(index);
                if (_filter(item)) continue;

                RemoveFromView(index);
                index--;
            }
        }

        var viewHash = new HashSet<object>(_view);

        int viewIndex = 0;
        for (int index = 0; index < _source.Count; index++)
        {
            var item = _source[index];
            if (viewHash.Contains(item))
            {
                viewIndex++;
                continue;
            }

            if (OnItemAdded(index, item, viewIndex))
                viewIndex++;
        }
    }

    public int Compare(object x, object y)
    {
        if (_groupDescription != null)
        {
            int result = _groupDescription.Compare(x, y);
            if (result != 0)
                return result;
        }

        foreach (var desc in _sortDescriptions)
        {
            int result = desc.Compare(x, y);
            if (result != 0)
                return result;
        }

        return 0;
    }

    public void Dispose()
    {
        _filter = null;

        _sortDescriptions.CollectionChanged -= OnSortDescriptionsChanged;
        if (_source is INotifyCollectionChanged ncc)
            ncc.CollectionChanged -= OnSourceCollectionChanged;
    }
}
