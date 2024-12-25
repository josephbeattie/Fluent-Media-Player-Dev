using System.ComponentModel;

namespace Rise.Effects
{
    public sealed class EqualizerBand : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _index;
        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Index)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HzText)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FormattedHz)));
                }
            }
        }

        private float _gain;
        public float Gain
        {
            get => _gain;
            set
            {
                if (_gain != value)
                {
                    _gain = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Gain)));
                }
            }
        }

        public string HzText
        {
            get
            {
                return Index switch
                {
                    0 => "30",
                    1 => "75",
                    2 => "150",
                    3 => "300",
                    4 => "300",
                    5 => "1.2k",
                    6 => "2.5k",
                    7 => "5k",
                    8 => "10k",
                    9 => "20k",
                    _ => string.Empty,
                };
            }
        }

        public string FormattedHz => $"{HzText}Hz";

        internal float Bandwidth { get; set; }
        internal float Frequency { get; set; }
    }
}
