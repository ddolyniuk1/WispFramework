using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WispFramework.Extensions
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

        public static async Task<IEnumerable<TResult>> ParallelSelectMany<TSource, TResult>(this IEnumerable<TSource> source, int chunkSize, Func<TSource, TResult> selector, CancellationToken cancellationToken, Action<int> inTaskOperation = null)
        {
            var chunks = source.ChunkBy(chunkSize).ToArray();
            var tasks = new Task<IEnumerable<TResult>>[chunks.Length];
              
            for (var index = 0; index < chunks.Count(); index++)
            {
                var item = chunks[index];
                var index1 = index;
                tasks[index] = Task.Factory.StartNew(list =>
                {
                    inTaskOperation?.Invoke(index1);
                    var li = (IEnumerable<TSource>) list;
                    return li.Select(selector); 
                }, item, cancellationToken); 
            }

            var results = await Task.WhenAll(tasks);

            return results.SelectMany(t => t); 
        }

        public static async Task<IEnumerable<TSource>> ParallelWhere<TSource>(this IEnumerable<TSource> source, int chunkSize, Func<TSource, bool> predicate, CancellationToken cancellationToken)
        {
            var chunks = source.ChunkBy(chunkSize).ToArray();
            var tasks = new Task<IEnumerable<TSource>>[chunks.Length];
              
            for (var index = 0; index < chunks.Count(); index++)
            {
                var item = chunks[index]; 
                tasks[index] = Task.Factory.StartNew(list =>
                { 
                    var li = (IEnumerable<TSource>) list;
                    return li.Where(predicate); 
                }, item, cancellationToken); 
            }

            var results = await Task.WhenAll(tasks);

            return results.SelectMany(t => t); 
        }
    }
}
