using Rise.App.Dialogs;
using Rise.App.ViewModels;
using Rise.Common.Extensions.Markup;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class PlaybackPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;
        private readonly List<string> Crossfade = new()
        {
            ResourceHelper.GetString("NoCrossfade")
        };

        private readonly List<string> VideoScale = new()
        {
            ResourceHelper.GetString("ScaleToWindow"),
            ResourceHelper.GetString("MatchResolution")
        };

        public PlaybackPage()
        {
            InitializeComponent();

            string format = ResourceHelper.GetString("NSeconds");

            Crossfade.Add(FormatSeconds("3"));
            Crossfade.Add(FormatSeconds("5"));
            Crossfade.Add(FormatSeconds("10"));

            string FormatSeconds(string sec)
                => string.Format(format, sec);
        }

        private async void OnEqualizerExpanderClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await new EqualizerDialog().ShowAsync();
        }
    }
}
