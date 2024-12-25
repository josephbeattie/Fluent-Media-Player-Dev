using Windows.UI;

namespace Rise.Models
{
    /// <summary>
    /// Defines a color with a name describing it.
    /// </summary>
    public readonly record struct NamedColor(string Name, Color Color)
    {
        public NamedColor(string Name, byte A, byte R, byte G, byte B)
            : this(Name, Color.FromArgb(A, R, G, B)) { }

        public NamedColor(string Name, byte R, byte G, byte B)
            : this(Name, Color.FromArgb(255, R, G, B)) { }
    }
}
