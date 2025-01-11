using System.Collections.Generic;

namespace Rise.Common.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets the key associated with the specified value.
        /// </summary>
        /// <param name="val">The value whose key to get.</param>
        /// <param name="key">When this method returns, the key associated with the specified value, if the
        /// value is found; otherwise, the default value for the type of the key parameter. This parameter is
        /// passed uninitialized.</param>
        /// <returns>true if the object that implements <see cref="IDictionary{TKey, TValue}"/> contains a
        /// key with the specified value; otherwise, false.</returns>
        public static bool TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TValue val, out TKey key)
        {
            key = default;
            foreach (KeyValuePair<TKey, TValue> pair in dict)
            {
                if (EqualityComparer<TValue>.Default.Equals(pair.Value, val))
                {
                    key = pair.Key;
                    return true;
                }
            }

            return false;
        }
    }
}
