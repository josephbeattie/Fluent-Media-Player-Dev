using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.Common.Attached
{
    /// <summary>
    /// A set of attached properties that make working with the
    /// grid system easier.
    /// </summary>
    public sealed class GridHelper : DependencyObject
    {
        /// <summary>
        /// A property that contains a string based representation
        /// of a grid's row heights.
        /// </summary>
        public static readonly DependencyProperty RowHeightsProperty =
            DependencyProperty.RegisterAttached("RowHeights", typeof(string),
                typeof(GridHelper), new PropertyMetadata(string.Empty, RowHeightsChanged));

        public static string GetRowHeights(Grid target)
            => (string)target.GetValue(RowHeightsProperty);
        public static void SetRowHeights(Grid target, string value)
            => target.SetValue(RowHeightsProperty, value);

        /// <summary>
        /// A property that contains a string based representation
        /// of a grid's column widths.
        /// </summary>
        public static readonly DependencyProperty ColumnWidthsProperty =
            DependencyProperty.RegisterAttached("ColumnWidths", typeof(string),
                typeof(GridHelper), new PropertyMetadata(string.Empty, ColumnWidthsChanged));

        public static string GetColumnWidths(Grid target)
            => (string)target.GetValue(ColumnWidthsProperty);
        public static void SetColumnWidths(Grid target, string value)
            => target.SetValue(ColumnWidthsProperty, value);

        private static void RowHeightsChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs args)
        {
            if (d is not Grid grid)
                return;

            grid.RowDefinitions.Clear();

            var definitions = args.NewValue.ToString();
            if (string.IsNullOrEmpty(definitions))
                return;

            var heights = definitions.Split(',');
            foreach (var height in heights)
                AddDefinition(grid, height, true);
        }

        private static void ColumnWidthsChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs args)
        {
            if (d is not Grid grid)
                return;

            grid.ColumnDefinitions.Clear();

            var definitions = args.NewValue.ToString();
            if (string.IsNullOrEmpty(definitions))
                return;

            var widths = definitions.Split(',');
            foreach (var width in widths)
                AddDefinition(grid, width, false);
        }

        public static void AddDefinition(Grid grid, string definition, bool isRow)
        {
            GridLength length;
            if (definition == "Auto")
            {
                length = GridLength.Auto;
            }
            else if (definition.EndsWith("*"))
            {
                var val = definition.Replace("*", "");
                if (string.IsNullOrEmpty(val))
                    val = "1";

                var size = double.Parse(val);
                length = new GridLength(size, GridUnitType.Star);
            }
            else
            {
                var size = double.Parse(definition);
                length = new GridLength(size, GridUnitType.Pixel);
            }

            if (isRow)
            {
                var def = new RowDefinition { Height = length };
                grid.RowDefinitions.Add(def);
            }
            else
            {
                var def = new ColumnDefinition { Width = length };
                grid.ColumnDefinitions.Add(def);
            }
        }
    }
}
