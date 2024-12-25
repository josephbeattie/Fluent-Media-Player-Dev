namespace Rise.Models
{
    public sealed class SearchItem
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ItemType { get; set; }
        public string Thumbnail { get; set; }

        public bool Equals(SearchItem other)
        {
            return Title == other.Title &&
                   Subtitle == other.Subtitle &&
                   ItemType == other.ItemType &&
                   Thumbnail == other.Thumbnail;
        }
    }
}
