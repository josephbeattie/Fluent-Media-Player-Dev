using System;
using System.Collections.Generic;
using System.Linq;

namespace Rise.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IList<T> CloneList<T>(this IEnumerable<T> source)
        {
            IList<T> list = new List<T>();

            foreach (var item in source)
            {
                list.Add(item);
            }

            return list;
        }

        public static IList<T2> CloneList<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> convert = null)
            where T2 : class
        {
            IList<T2> list = new List<T2>();

            convert ??= (t1) => t1 as T2;

            foreach (var item in source)
            {
                list.Add(convert(item));
            }

            return list;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rnd)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (rnd == null) throw new ArgumentNullException(nameof(rnd));

            return ShuffleIterator<T>(source, rnd);
        }

        private static IEnumerable<T> ShuffleIterator<T>(IEnumerable<T> source, Random rnd)
        {
            IList<T> buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rnd.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
    }
}
