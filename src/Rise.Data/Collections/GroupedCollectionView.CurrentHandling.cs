using Windows.UI.Xaml.Data;

namespace Rise.Data.Collections;

public sealed partial class GroupedCollectionView
{
    public object CurrentItem
    {
        get
        {
            if (CurrentPosition > -1 && CurrentPosition < _view.Count)
                return _view[CurrentPosition];
            return null;
        }
    }
    public int CurrentPosition { get; private set; }

    public bool IsCurrentBeforeFirst => CurrentPosition < 0;
    public bool IsCurrentAfterLast => CurrentPosition >= _view.Count;

    public bool MoveCurrentTo(object item)
    {
        if (item == CurrentItem)
            return true;
        return MoveCurrentToIndex(_view.IndexOf(item));
    }

    public bool MoveCurrentToPosition(int index)
        => MoveCurrentToIndex(index);

    public bool MoveCurrentToFirst()
        => MoveCurrentToIndex(0);

    public bool MoveCurrentToLast()
        => MoveCurrentToIndex(_view.Count - 1);

    public bool MoveCurrentToNext()
        => MoveCurrentToIndex(CurrentPosition + 1);

    public bool MoveCurrentToPrevious()
        => MoveCurrentToIndex(CurrentPosition - 1);

    private bool MoveCurrentToIndex(int index)
    {
        if (index < -1 || index >= _view.Count)
            return false;

        var args = new CurrentChangingEventArgs();
        OnCurrentChanging(args);

        if (args.Cancel)
            return false;

        CurrentPosition = index;
        OnCurrentChanged();

        return true;
    }
}
