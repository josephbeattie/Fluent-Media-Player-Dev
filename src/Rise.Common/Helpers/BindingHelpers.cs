using Windows.UI.Xaml;

namespace Rise.Common.Helpers
{
    public static class BindingHelpers
    {
        /// <summary>
        /// If all the provided values are true, this method
        /// returns <see cref="Visibility.Visible"/>. Otherwise,
        /// it returns <see cref="Visibility.Collapsed"/>
        /// </summary>
        public static Visibility BooleansToVisibility(bool first, bool second)
        {
            if (first && second)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        /// <summary>
        /// If all the provided values are true, this method
        /// returns <see cref="Visibility.Collapsed"/>. Otherwise,
        /// it returns <see cref="Visibility.Visible"/>
        /// </summary>
        public static Visibility InverseBooleansToVisibility(bool first, bool second)
        {
            if (first && second)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public static bool IsNotNull(object obj)
        {
            return obj != null;
        }

        public static bool ObjEquals(object obj, object otherObj)
        {
            return obj == otherObj;
        }

        public static Visibility BoolToVis(bool boolean)
            => boolean ? Visibility.Visible : Visibility.Collapsed;

        public static Visibility InverseBoolToVis(bool boolean)
            => !boolean ? Visibility.Visible : Visibility.Collapsed;

        public static Visibility IsNotNullVis(object obj)
            => BoolToVis(obj != null);

        public static bool IsNull(object obj)
        {
            return obj == null;
        }

        public static bool Reverse(bool boolean)
        {
            return !boolean;
        }

        public static string ConcatString(string str1, string str2)
        {
            return $"{str1} {str2}";
        }

        public static string ConcatString(int integer, string str2)
        {
            return $"{integer} {str2}";
        }
    }
}
