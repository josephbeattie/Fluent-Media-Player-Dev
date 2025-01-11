﻿using System;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed class NullToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return parameter == null ? value != null : parameter.ToString() == "Reverse" ? value == null : (object)(value != null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
