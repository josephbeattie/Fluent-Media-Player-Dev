namespace Rise.Models
{
    public sealed class Wallpaper
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }

        public override string ToString() => Name;
    }
}
