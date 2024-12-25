using Rise.Models;
using System;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Rise.App.Helpers
{
    // Thanks to @ahmed605 (https://github.com/ahmed605) for the Musixmatch code contribution.
    public static class MusixmatchHelper
    {
        private static async Task<string> GetStringAsync(Uri url)
        {
            using HttpClient httpClient = new();
            try
            {
                using var response = await httpClient.GetAsync(url);
                _ = response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }

        private static string GetUserToken(UserToken token)
        {
            if (token != null)
            {
                var message = token.Message;
                string userToken = message.Body.Token;

                if (message.Header.StatusCode == 200 && !string.IsNullOrEmpty(userToken))
                    return userToken;
            }

            return "2306258e8a658a197b52f987c6f83479b4cce70202a27357e58b68";
        }

        public static async Task<MusixmatchLyrics> GetLyricsAsync(string trackName, string artistName, UserToken token = null)
        {
            string userToken = GetUserToken(token);
            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/matcher.lyrics.get?format=json&q_track={Uri.EscapeDataString(trackName)}&q_artist={Uri.EscapeDataString(artistName)}&user_language=en&subtitle_format=lrc&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);
            if (!string.IsNullOrWhiteSpace(response))
                return MusixmatchLyrics.FromJson(response);

            return null;
        }

        // Duration is in seconds
        public static async Task<MusixmatchLyrics> GetLyricsAsync(string trackName, string artistName, int duration, UserToken token = null)
        {
            string userToken = GetUserToken(token);
            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/matcher.lyrics.get?format=json&q_track={Uri.EscapeDataString(trackName)}&q_artist={Uri.EscapeDataString(artistName)}&q_duration={duration}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);
            if (!string.IsNullOrWhiteSpace(response))
                return MusixmatchLyrics.FromJson(response);

            return null;
        }

        public static async Task<MusixmatchLyrics> GetLyricsAsync(string id, UserToken token = null)
        {
            string userToken = GetUserToken(token);
            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/track.lyrics.get?format=json&track_id={id}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);
            if (!string.IsNullOrWhiteSpace(response))
                return MusixmatchLyrics.FromJson(response);

            return null;
        }

        public static async Task<SyncedLyrics> GetSyncedLyricsAsync(string trackName, string artistName, UserToken token = null)
        {
            string userToken = GetUserToken(token);
            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/matcher.subtitle.get?format=json&q_track={Uri.EscapeDataString(trackName)}&q_artist={Uri.EscapeDataString(artistName)}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);
            if (!string.IsNullOrWhiteSpace(response))
                return SyncedLyrics.FromJson(response);

            return null;
        }

        // Duration is in seconds
        public static async Task<SyncedLyrics> GetSyncedLyricsAsync(string trackName, string artistName, int duration, UserToken token = null)
        {
            string userToken = GetUserToken(token);
            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/matcher.subtitle.get?format=json&q_track={Uri.EscapeDataString(trackName)}&q_artist={Uri.EscapeDataString(artistName)}&q_duration={duration}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);
            if (!string.IsNullOrWhiteSpace(response))
                return SyncedLyrics.FromJson(response);

            return null;
        }

        public static async Task<SyncedLyrics> GetSyncedLyricsAsync(string id, UserToken token = null)
        {
            string userToken = GetUserToken(token);
            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/track.subtitle.get?format=json&track_id={id}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);
            if (!string.IsNullOrWhiteSpace(response))
                return SyncedLyrics.FromJson(response);

            return null;
        }

        public static async Task<MusixmatchTrack> GetTrackAsync(string trackName, string artistName, UserToken token = null)
        {
            string userToken = GetUserToken(token);
            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/matcher.track.get?format=json&q_track={Uri.EscapeDataString(trackName)}&q_artist={Uri.EscapeDataString(artistName)}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);
            if (!string.IsNullOrWhiteSpace(response))
                return MusixmatchTrack.FromJson(response);

            return null;
        }

        // Duration is in seconds
        public static async Task<MusixmatchTrack> GetTrackAsync(string trackName, string artistName, int duration, UserToken token = null)
        {
            string userToken = GetUserToken(token);
            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/matcher.track.get?format=json&q_track={Uri.EscapeDataString(trackName)}&q_artist={Uri.EscapeDataString(artistName)}&q_duration={duration}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);
            if (!string.IsNullOrWhiteSpace(response))
                return MusixmatchTrack.FromJson(response);

            return null;
        }

        public static async Task<UserToken> GetUserTokenAsync()
        {
            Uri url = new(@"https://apic.musixmatch.com/ws/1.1/token.get?app_id=community-app-v1.0&guid=56495545-24fd-4a2f-95dc-947a250c4550&signature=NlaOMebAhGJEjN0zRTwyD%2FSspUo%3D&signature_protocol=sha1");

            string response = await GetStringAsync(url);
            if (!string.IsNullOrEmpty(response))
                return UserToken.FromJson(response);

            return null;
        }
    }
}
