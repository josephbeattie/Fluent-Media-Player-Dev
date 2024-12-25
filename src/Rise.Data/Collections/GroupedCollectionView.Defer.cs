using Windows.Foundation;

namespace Rise.Data.Collections;

public sealed partial class GroupedCollectionView
{
    private uint _deferCounter = 0;

    /// <summary>
    /// Creates a deferral that prevents the view from listening
    /// to changes until it completes.
    /// </summary>
    /// <remarks>This only defers collection changes raised on
    /// <see cref="GroupedCollectionView"/>. When all deferrals
    /// complete, the collection is fully refreshed, and a
    /// vector change is invoked using the reset action.</remarks>
    public Deferral DeferRefresh()
    {
        _deferCounter++;
        return new Deferral(OnDeferralCompleted);
    }

    private void OnDeferralCompleted()
    {
        _deferCounter--;
        if (_deferCounter == 0)
            OnSourceChanged();
    }
}
