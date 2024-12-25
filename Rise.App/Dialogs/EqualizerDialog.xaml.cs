using Rise.App.ViewModels;
using Rise.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Rise.App.Dialogs
{
    public sealed partial class EqualizerDialog : ContentDialog
    {
        private SettingsViewModel SViewModel => App.SViewModel;
        private EqualizerEffect Effect => EqualizerEffect.Current;

        private readonly int _initialPreset;
        private int _currPreset;

        public EqualizerDialog()
        {
            if (!EqualizerEffect.Initialized)
            {
                var eq = EqualizerEffect.Current;
                eq.InitializeBands(SViewModel.EqualizerGain);
                eq.IsEnabled = SViewModel.EqualizerEnabled;
            }

            InitializeComponent();

            _initialPreset = SViewModel.SelectedEqualizerPreset;
            _currPreset = _initialPreset;
        }
    }

    // Event handlers
    public sealed partial class EqualizerDialog
    {
        private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            SViewModel.EqualizerEnabled = Effect.IsEnabled;

            var gains = new float[10];
            for (int i = 0; i < 10; i++)
                gains[i] = Effect.Bands[i].Gain;

            SViewModel.EqualizerGain = gains;
            Effect.UpdateAllBands();
        }

        private void OnSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Effect.IsEnabled = SViewModel.EqualizerEnabled;

            Presets.SelectionChanged -= OnPresetChanged;
            SViewModel.SelectedEqualizerPreset = _initialPreset;

            for (int i = 0; i < 10; i++)
                Effect.Bands[i].Gain = SViewModel.EqualizerGain[i];
        }

        private void OnPresetChanged(object sender, SelectionChangedEventArgs e)
        {
            int newPreset = (sender as ComboBox).SelectedIndex;
            if (_currPreset == newPreset)
                return;

            _currPreset = newPreset;

            float[] gains = new float[10];
            switch (newPreset)
            {
                case 0:
                    gains = new float[10] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
                    break;
                case 1:
                    return;
                case 2:
                    gains = new float[10] { 6f, 4f, 2f, 0f, 2f, 3f, 0f, 0f, 0f, 0f };
                    break;
                case 3:
                    gains = new float[10] { 4f, 3f, 2f, 1f, 0f, -1f, 0f, 1f, 2f, 3f };
                    break;
                case 4:
                    gains = new float[10] { 0f, 0f, 0f, 0f, 2f, 2f, 1f, 0f, 0f, 0f };
                    break;
                case 5:
                    gains = new float[10] { 4f, 3f, 2f, 1f, 0f, 0f, 0f, 0f, 0f, 0f };
                    break;
                case 6:
                    gains = new float[10] { 0f, 0f, 0f, 0f, 0f, 0f, 1f, 2f, 3f, 4f };
                    break;
            }

            for (int i = 0; i < 10; i++)
                Effect.Bands[i].Gain = gains[i];
        }

        private void OnBandGainChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var elm = sender as FrameworkElement;
            if (elm?.DataContext is EqualizerBand band)
                Effect.UpdateBand(band.Index);
        }
    }
}
