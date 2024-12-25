using System.Runtime.InteropServices;

namespace Rise.Common.Helpers
{
    public static class PathUtils
    {
        [DllImport("shlwapi.dll", ExactSpelling = true)]
        public static extern bool PathIsNetworkPathW(string pszPath);
    }
}
