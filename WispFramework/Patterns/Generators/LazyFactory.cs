using System;
using System.Collections.Concurrent;
using System.Data;

namespace WispFramework.Patterns.Generators
{
    /// <summary>
    ///     Try and get object if it has already been created, otherwise we initialize a new object and return it
    /// </summary>
    /// <typeparam name="TKeyType"></typeparam>
    /// <typeparam name="TResultType"></typeparam>
    public class LazyFactory<TKeyType, TResultType>
    {
        private readonly object _syncLock = new object();

        public LazyFactory(Func<TKeyType, TResultType> initializer)
        {
            Initializer = initializer;
        }

        public LazyFactory()
        {
        }

        /// <summary>
        ///     Retrieve an object by key, initializing a new one if it does not already exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TResultType this[TKeyType key]
        {
            get
            {
                if (InternalCache.TryGetValue(key, out var result))
                {
                    return result;
                }

                if (Initializer == null)
                {
                    throw new EvaluateException(
                        "Initializer was never set for this object. Please set it before requesting an object within the cache.");
                }

                lock (_syncLock)
                {
                    var newThrottled = Initializer.Invoke(key);
                    InternalCache[key] = newThrottled;
                    return newThrottled;
                }
            }
        }

        public Func<TKeyType, TResultType> Initializer { get; set; }

        public ConcurrentDictionary<TKeyType, TResultType> InternalCache { get; } =
            new ConcurrentDictionary<TKeyType, TResultType>();

        public void Clear()
        {
            InternalCache.Clear();
        }

        /// <summary>
        ///     Retrieve an object by key, initializing a new one if it does not already exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TResultType Get(TKeyType key)
        {
            return this[key];
        }

        public LazyFactory<TKeyType, TResultType> SetInitializer(Func<TKeyType, TResultType> initializer)
        {
            Initializer = initializer;
            return this;
        }

        /// <summary>
        ///     Try to get existing object without initializing a new one if it is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetValue(TKeyType key, out TResultType result)
        {
            var ret = InternalCache.TryGetValue(key, out var found);
            result = found;
            return ret;
        }

        /// <summary>
        ///     Attempts to remove object from internal cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool TryRemove(TKeyType key)
        {
            var removed = InternalCache.TryRemove(key, out var val);
            return removed;
        }
    }
}