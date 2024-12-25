namespace Rise.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Gets substring until the first occurrence of
        /// <paramref name="stopAt"/>.
        /// </summary>
        /// <returns>true if <paramref name="stopAt"/> is found beyond
        /// the beginning of the string, false otherwise.</returns>
        public static bool TryGetUntil(this string text, char stopAt, out string result)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt);
                if (charLocation > 0)
                {
                    result = text.Remove(charLocation);
                    return true;
                }
            }

            result = string.Empty;
            return false;
        }

        /// <summary>
        /// Replaces the given string if it's null, empty, or
        /// consists only of white-space characters.
        /// </summary>
        /// <param name="initial">string to check.</param>
        /// <param name="replacement">Replacement to use if
        /// needed.</param>
        /// <returns>The string that should be used.</returns>
        public static string ReplaceIfNullOrWhiteSpace(this string initial, string replacement)
        {
            if (string.IsNullOrWhiteSpace(initial))
                return replacement;
            return initial;
        }
    }
}
