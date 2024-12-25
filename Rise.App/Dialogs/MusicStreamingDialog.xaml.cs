using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using System;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace Rise.App.Dialogs
{
    public sealed partial class MusicStreamingDialog : ContentDialog
    {
        private MediaPlaybackViewModel ViewModel => App.MPViewModel;

        public MusicStreamingDialog()
        {
            InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();

            string url = StreamingTextBox.Text;
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                args.Cancel = true;
                InvalidUrlText.Visibility = Visibility.Visible;
                return;
            }

            MediaPlaybackItem song;

            bool isYoutubeLink = url.Contains("youtube.com/watch");
            if (isYoutubeLink)
            {
                var youtubeClient = new YoutubeClient();
                var youtubeVideo = await youtubeClient.Videos.GetAsync(url.Replace("music.youtube.com", "www.youtube.com"));

                string title = youtubeVideo.Title;
                string subtitle = youtubeVideo.Author.ChannelTitle;
                string thumbnailUrl = youtubeVideo.Thumbnails.GetWithHighestResolution().Url;

                var streams = await youtubeClient.Videos.Streams.GetManifestAsync(url);

                uri = new(streams.GetAudioStreams().GetWithHighestBitrate().Url);
                song = await WebHelpers.GetSongFromUriAsync(uri, title, subtitle, thumbnailUrl);
            }
            else
            {
                song = await WebHelpers.GetSongFromUriAsync(uri);
            }

            deferral?.Complete();

            await ViewModel.ResetPlaybackAsync();
            ViewModel.AddSingleItemToQueue(song);
            ViewModel.Player.Play();
        }
    }
}
