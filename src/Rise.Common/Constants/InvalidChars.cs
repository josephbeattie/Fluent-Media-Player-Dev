namespace Rise.Common.Constants
{
    public class InvalidChars
    {
        /// <summary>
        /// List of invalid characters in a filename.
        /// </summary>
        public static char[] Invalids = new char[] { '"', '<', '>', '|',
            '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005',
            '\x0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r',
            '\x000e', '\x000f', '\x0010', '\x0011', '\x0012',
            '\x0013', '\x0014', '\x0015', '\x0016', '\x0017',
            '\x0018', '\x0019', '\x001a', '\x001b', '\x001c',
            '\x001d', '\x001e', '\x001f', ':', '*', '?', '\\', '/' };
    }
}
