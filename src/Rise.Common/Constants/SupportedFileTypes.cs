using System.Linq;

namespace Rise.Common.Constants
{
    public static class SupportedFileTypes
    {
        public static readonly string[] MusicFiles = new string[]
        {
            ".mp3", ".wma", ".wav", ".ogg", ".flac", ".aiff", ".aac", ".m4a", ".wm", ".3gp", ".3gp2"
        };

        public static readonly string[] PlaylistFiles = new string[]
        {
            ".m3u", ".m3u8", ".wpl", ".zpl"
        };

        public static readonly string[] VideoFiles = new string[]
        {
            ".m2v", ".m4v", ".mp4", ".mov", ".asf", ".avi", ".wmv", ".mkv", ".mp4v", ".mod", ".wm", ".mpg4", ".mpv2", ".ogm", ".ogv", ".mpeg", ".mpg", ".ogx", ".mpe", ".m1v", ".m2ts"
        };

        private static string[] mediaFiles;

        public static string[] MediaFiles
            => mediaFiles ??= MusicFiles.Concat(VideoFiles).ToArray();
    }
}
