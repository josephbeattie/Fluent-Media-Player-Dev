using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rise.Data.Navigation
{
    /// <summary>
    /// A collection of items to be displayed in a NavigationView. For binding
    /// purposes, it exposes observable views for main and footer items, and
    /// handles external modifications as needed.
    /// </summary>
    /// <remarks>This collection isn't observable on its own - the menu and
    /// footer views are, despite being read-only.</remarks>
    public sealed class NavigationItemCollection : IList<NavigationItemBase>
    {
        public NavigationItemBase this[int index]
        {
            get
            {
                int menuCount = _menuItems.Count;
                if (index >= menuCount)
                    return _footerItems[index - menuCount];
                return _menuItems[index];
            }
            set
            {
                int menuCount = _menuItems.Count;
                if (index >= menuCount)
                    _footerItems[index - menuCount] = value;
                else
                    _menuItems[index] = value;
            }
        }

        private readonly ObservableCollection<NavigationItemBase> _menuItems;
        /// <summary>
        /// Gets an observable view of the items in the main section.
        /// </summary>
        public ReadOnlyObservableCollection<NavigationItemBase> MenuItems { get; }

        private readonly ObservableCollection<NavigationItemBase> _footerItems;
        /// <summary>
        /// Gets an observable view of the items in the footer section.
        /// </summary>
        public ReadOnlyObservableCollection<NavigationItemBase> FooterItems { get; }

        public int Count => _menuItems.Count + _footerItems.Count;
        public bool IsReadOnly => false;

        public NavigationItemCollection()
        {
            _menuItems = new();
            _footerItems = new();

            MenuItems = new(_menuItems);
            FooterItems = new(_footerItems);
        }

        public NavigationItemCollection(IEnumerable<NavigationItemBase> items)
        {
            _menuItems = new(items.Where(i => !i.IsFooter));
            _footerItems = new(items.Where(i => i.IsFooter));

            MenuItems = new(_menuItems);
            FooterItems = new(_footerItems);
        }

        /// <summary>
        /// Returns the footer items collection if getFooter is true,
        /// otherwise returns the main one.
        /// </summary>
        public ReadOnlyObservableCollection<NavigationItemBase> GetView(bool getFooter)
        {
            if (getFooter)
                return FooterItems;
            return MenuItems;
        }

        public int IndexOf(NavigationItemBase item)
        {
            if (item.IsFooter)
                return _footerItems.IndexOf(item) + _menuItems.Count;
            return _menuItems.IndexOf(item);
        }

        public void Insert(int index, NavigationItemBase item)
        {
            if (item.IsFooter)
                _footerItems.Insert(index - _menuItems.Count, item);
            else
                _menuItems.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            int menuCount = _menuItems.Count;
            if (index >= menuCount)
                _footerItems.RemoveAt(index - menuCount);
            else
                _menuItems.RemoveAt(index);
        }

        public void Add(NavigationItemBase item)
        {
            if (item.IsFooter)
                _footerItems.Add(item);
            else
                _menuItems.Add(item);
        }

        public void Clear()
        {
            _menuItems.Clear();
            _footerItems.Clear();
        }

        public bool Contains(NavigationItemBase item)
        {
            if (item.IsFooter)
                return _footerItems.Contains(item);
            return _menuItems.Contains(item);
        }

        public void CopyTo(NavigationItemBase[] array, int arrayIndex)
        {
            _menuItems.CopyTo(array, arrayIndex);
            _footerItems.CopyTo(array, arrayIndex + _menuItems.Count);
        }

        public bool Remove(NavigationItemBase item)
        {
            if (item.IsFooter)
                return _footerItems.Remove(item);
            return _menuItems.Remove(item);
        }

        public IEnumerator<NavigationItemBase> GetEnumerator()
            => _menuItems.Concat(_footerItems).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _menuItems.Concat(_footerItems).GetEnumerator();
    }

    [JsonSerializable(typeof(IEnumerable<NavigationItemBase>))]
    internal sealed partial class NavigationItemCollectionContext : JsonSerializerContext
    {
    }
}
