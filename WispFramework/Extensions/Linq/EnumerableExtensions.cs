using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WispFramework.Utility;

namespace WispFramework.Extensions.Linq
{
    /// <summary>
    /// Helper methods for the lists.
    /// </summary>
    public static class ListExtensions
    {
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize) 
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        } 

        public static string ToCsv<T>(this IEnumerable<T> list)
        {
            return list.Stringify().Join(", ");
        }

        public static string Join<T>(this IEnumerable<T> list, string separator, Func<T, string> toStringExpression)
        {
            return list.Stringify(toStringExpression).Join(separator);
        }

        public static IEnumerable<string> Stringify<T>(this IEnumerable<T> list)
        {
            return list.Select(t => t.ToString());
        }

        public static IEnumerable<string> Stringify<T>(this IEnumerable<T> list, Func<T, string> toStringExpression)
        {
            return list.Select(toStringExpression);
        }

        public static string Join(this IEnumerable<string> list, string separator)
        {
            return string.Join(separator, list);
        }

        public static T RandomItem<T>(this IEnumerable<T> list)
        {
            var enumerable = list as T[] ?? list.ToArray();
            return !enumerable.Any() ? default : enumerable.ElementAtOrDefault(RandomUtil.Range(0, enumerable.Count()));
        } 

        /// <summary>
        /// Shuffle the IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            var enumerable = list as T[] ?? list.ToArray();
            var shuffledList =
                enumerable.Select(x => new {Number = RandomUtil.Range(0, enumerable.Count()), Item = x}).OrderBy(x => x.Number).Select(x => x.Item); 
            return shuffledList.ToList();
        }
    }
}
