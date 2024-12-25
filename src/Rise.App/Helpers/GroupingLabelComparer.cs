using System.Collections;
using System.Collections.Generic;

namespace Rise.App.Helpers;

/// <summary>
/// A comparer to preserve the original order of the labels in an instance of
/// <see cref="Windows.Globalization.Collation.CharacterGroupings"/>.
/// </summary>
public sealed class GroupingLabelComparer : IComparer, IComparer<string>
{
    public static readonly GroupingLabelComparer Default = new();
    private GroupingLabelComparer() { }

    public int Compare(object x, object y)
        => Compare(x.ToString(), y.ToString());

    public int Compare(string x, string y)
    {
        if (x == y)
            return 0;

        if (string.IsNullOrEmpty(x))
            return 1;
        else if (string.IsNullOrEmpty(y))
            return -1;
        else if (x == "&" && y == "#")
            return -1;
        else if (x == "#" && y == "&")
            return 1;

        return x.CompareTo(y);
    }
}
