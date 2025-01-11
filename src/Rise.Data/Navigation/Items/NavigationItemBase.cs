using Rise.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rise.Data.Navigation
{
    /// <summary>
    /// A base class for items to be placed in a NavigationView.
    /// </summary>
    [JsonDerivedType(typeof(NavigationItemSeparator), (int)NavigationItemType.Separator)]
    [JsonDerivedType(typeof(NavigationItemHeader), (int)NavigationItemType.Header)]
    [JsonDerivedType(typeof(NavigationItemDestination), (int)NavigationItemType.Destination)]
    public abstract class NavigationItemBase : ViewModel, IEquatable<NavigationItemBase>
    {
        /// <summary>
        /// Indicates the type of item.
        /// </summary>
        public NavigationItemType ItemType { get; }

        protected NavigationItemBase(NavigationItemType itemType)
        {
            ItemType = itemType;
        }

        /// <summary>
        /// A unique identifier for this item.
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// The unique identifier for this item's parent. Only applies
        /// if this item is contained inside another one.
        /// </summary>
        public string ParentId { get; init; }

        /// <summary>
        /// An identifier for the group this item belongs to.
        /// </summary>
        public string Group { get; init; }

        /// <summary>
        /// Whether the item should be in the footer.
        /// </summary>
        public bool IsFooter { get; init; }

        private bool _isVisible = true;
        /// <summary>
        /// Whether the item should be visible.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NavigationItemBase);
        }

        public bool Equals(NavigationItemBase other)
        {
            return other is not null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
        }
    }
}
