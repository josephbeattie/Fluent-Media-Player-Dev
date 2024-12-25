using Rise.App.ViewModels;
using Rise.Common.Enums;
using Rise.Common.Extensions.Markup;
using Rise.Models;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class AppearancePage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        private readonly List<string> Themes = new()
        {
            ResourceHelper.GetString("Light"),
            ResourceHelper.GetString("Dark"),
            ResourceHelper.GetString("UseSystemSetting")
        };

        private readonly List<string> ColorThemes = new()
        {
            ResourceHelper.GetString("/Settings/AppearanceNoGlaze"),
            ResourceHelper.GetString("/Settings/AppearanceSystemGlazeColor"),
            ResourceHelper.GetString("/Settings/AppearanceCustomGlazeColor"),
            ResourceHelper.GetString("/Settings/AppearanceGlazeAlbumArt")
        };

        private static List<NamedColor> _glazeColors;
        private List<NamedColor> GlazeColors => _glazeColors;

        public AppearancePage()
        {
            InitializeComponent();

            if (_glazeColors == null)
            {
                _glazeColors = new();
                PopulateColors();
            }
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            ThemeChange.SelectionChanged += ThemeChange_SelectionChanged;
            ColorThemeComboBox.SelectionChanged += ColorThemeComboBox_SelectionChanged;

            ColorGrid.SelectionChanged += ColorGrid_SelectionChanged;
        }

        private void PopulateColors()
        {
            AddColor(255, 185, 0);
            AddColor(255, 140, 0);
            AddColor(247, 99, 12);
            AddColor(202, 80, 16);
            AddColor(218, 59, 1);
            AddColor(239, 105, 80);
            AddColor(209, 52, 56);
            AddColor(255, 67, 67);
            AddColor(231, 72, 86);
            AddColor(232, 17, 35);
            AddColor(234, 0, 94);
            AddColor(195, 0, 82);
            AddColor(227, 0, 140);
            AddColor(191, 0, 119);
            AddColor(194, 57, 179);
            AddColor(154, 0, 137);
            AddColor(0, 120, 212);
            AddColor(0, 99, 177);
            AddColor(142, 140, 216);
            AddColor(107, 105, 214);
            AddColor(135, 100, 184);
            AddColor(116, 77, 169);
            AddColor(177, 70, 194);
            AddColor(136, 23, 152);
            AddColor(0, 153, 188);
            AddColor(45, 125, 154);
            AddColor(0, 183, 195);
            AddColor(3, 131, 135);
            AddColor(0, 178, 148);
            AddColor(1, 133, 116);
            AddColor(0, 204, 106);
            AddColor(16, 137, 62);
            AddColor(122, 117, 116);
            AddColor(93, 90, 88);
            AddColor(104, 118, 138);
            AddColor(81, 92, 107);
            AddColor(86, 124, 116);
            AddColor(72, 104, 96);
            AddColor(73, 130, 5);
            AddColor(16, 124, 16);
            AddColor(118, 118, 118);
            AddColor(76, 74, 72);
            AddColor(105, 121, 126);
            AddColor(74, 84, 89);
            AddColor(100, 124, 100);
            AddColor(82, 94, 84);
            AddColor(132, 117, 69);
            AddColor(126, 115, 95);

            static void AddColor(byte r, byte g, byte b)
            {
                var color = Color.FromArgb(255, r, g, b);
                string name = ColorHelper.ToDisplayName(color);

                _glazeColors.Add(new(name, color));
            }
        }
    }

    // Event handlers
    public sealed partial class AppearancePage
    {
        private void ThemeChange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeThemeTip.IsOpen = true;
        }

        private void ColorThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var glaze = (GlazeTypes)ColorThemeComboBox.SelectedIndex;
            if (ViewModel.SelectedGlaze == glaze)
                return;

            ViewModel.SelectedGlaze = glaze;
            switch (glaze)
            {
                case GlazeTypes.None:
                    ViewModel.GlazeColors = Colors.Transparent;
                    break;

                case GlazeTypes.AccentColor:
                    var uiSettings = new UISettings();
                    var accent = uiSettings.GetColorValue(UIColorType.Accent);
                    accent.A = 25;

                    ViewModel.GlazeColors = accent;
                    break;

                case GlazeTypes.CustomColor:
                    var color = (NamedColor)ColorGrid.SelectedItem;
                    var col = color.Color;

                    ViewModel.GlazeColors = Color.FromArgb(25, col.R, col.G, col.B);
                    break;
            }
        }

        private void ColorGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var color = (NamedColor)e.AddedItems[0];
            var col = color.Color;

            ViewModel.GlazeColors = Color.FromArgb(25, col.R, col.G, col.B);
        }

        private async void ChangeThemeTip_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
            => _ = await CoreApplication.RequestRestartAsync("ThemeChanged");
    }
}
