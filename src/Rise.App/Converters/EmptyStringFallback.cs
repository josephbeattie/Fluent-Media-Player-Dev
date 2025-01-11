﻿using System;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters;

/// <summary>
/// Returns the provided value if it's not a null or empty string
/// or equal to 0, returns the converter parameter otherwise.
/// </summary>
public sealed class EmptyStringFallback : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        string str = value?.ToString();
        return string.IsNullOrEmpty(str) || str == "0" ? parameter : str;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
