using Windows.UI.Xaml.Data;

namespace Rise.Data.Collections;

public sealed partial class GroupedCollectionView
{
    public object CurrentItem => CurrentPosition > -1 && CurrentPosition < _view.Count ? _view[CurrentPosition] : null;
    public int CurrentPosition { get; private set; }

    public bool IsCurrentBeforeFirst => CurrentPosition < 0;
    public bool IsCurrentAfterLast => CurrentPosition >= _view.Count;

    public bool MoveCurrentTo(object item)
    {
        return item == CurrentItem || MoveCurrentToIndex(_view.IndexOf(item));
    }

    public bool MoveCurrentToPosition(int index)
    {
        return MoveCurrentToIndex(index);
    }

    public bool MoveCurrentToFirst()
    {
        return MoveCurrentToIndex(0);
    }

    public bool MoveCurrentToLast()
    {
        return MoveCurrentToIndex(_view.Count - 1);
    }

    public bool MoveCurrentToNext()
    {
        return MoveCurrentToIndex(CurrentPosition + 1);
    }

    public bool MoveCurrentToPrevious()
    {
        return MoveCurrentToIndex(CurrentPosition - 1);
    }

    private bool MoveCurrentToIndex(int index)
    {
        if (index < -1 || index >= _view.Count)
        {
            return false;
        }

        CurrentChangingEventArgs args = new();
        OnCurrentChanging(args);

        if (args.Cancel)
        {
            return false;
        }

        CurrentPosition = index;
        OnCurrentChanged();

        return true;
    }
}
