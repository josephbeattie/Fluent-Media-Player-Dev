using Rise.Common.Helpers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;

namespace Rise.Data.Collections;

public sealed partial class GroupedCollectionView
{
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new(propertyName));

    // Current item
    public event CurrentChangingEventHandler CurrentChanging;
    private void OnCurrentChanging(CurrentChangingEventArgs args)
        => CurrentChanging?.Invoke(this, args);

    public event EventHandler<object> CurrentChanged;
    private void OnCurrentChanged()
        => CurrentChanged?.Invoke(this, null);

    // Observable vector
    public event VectorChangedEventHandler<object> VectorChanged;
    private void OnVectorChanged(CollectionChange change, uint index)
        => VectorChanged?.Invoke(this, new VectorChangedEventArgs(change, index));
}
