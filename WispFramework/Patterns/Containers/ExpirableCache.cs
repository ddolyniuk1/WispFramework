using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WispFramework.Patterns.Generators;
using WispFramework.Patterns.Observables;

namespace WispFramework.Patterns.Containers
{
    public class ExpirableCache<TKeyType, TValueType> : IEnumerable<KeyValuePair<TKeyType, TValueType>>
    {
        public ExpirableCache(TimeSpan? itemLifespan = null)
        {
            if (!itemLifespan.HasValue)
            {
                itemLifespan = TimeSpan.FromSeconds(5);
            }

            LifeSpan = itemLifespan.Value;

            Snapshot = new Dynamic<IEnumerable<KeyValuePair<TKeyType, TValueType>>>(GetSnapshot)
                .Throttle(TimeSpan.FromSeconds(0.2f));
        }

        public ConcurrentDictionary<TKeyType, Expirable<TValueType>> InternalDictionary { get; } =
            new ConcurrentDictionary<TKeyType, Expirable<TValueType>>();

        private Dynamic<IEnumerable<KeyValuePair<TKeyType, TValueType>>> Snapshot { get; }


        public TimeSpan LifeSpan { get; set; }

        public int Count
        {
            get { return InternalDictionary.Values.Count(t => !t.IsExpired); }
        }

        public TValueType this[TKeyType key]
        {
            get => InternalDictionary[key];
            set => InternalDictionary[key] = new Expirable<TValueType>(value, LifeSpan);
        }

        public ICollection<TKeyType> Keys => Snapshot.Value.Select(t => t.Key).ToList();

        public ICollection<TValueType> Values => Snapshot.Value.Select(t => t.Value).ToList();

        public IEnumerator<KeyValuePair<TKeyType, TValueType>> GetEnumerator()
        {
            return Snapshot.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ExpirableCache<TKeyType, TValueType> WithSnapshotRefreshInterval(TimeSpan newRefreshTimespan)
        {
            Snapshot.Throttle(newRefreshTimespan);
            return this;
        }

        public ExpirableCache<TKeyType, TValueType> WithLifespan(TimeSpan newLifespan)
        {
            LifeSpan = newLifespan;
            return this;
        }

        private IEnumerable<KeyValuePair<TKeyType, TValueType>> GetSnapshot()
        {
            return InternalDictionary.Where(t => t.Value != null && !t.Value.IsExpired)
                .Select(i => new KeyValuePair<TKeyType, TValueType>(i.Key, i.Value.Value));
        }

        public void Add(KeyValuePair<TKeyType, TValueType> item)
        {
            InternalDictionary.TryAdd(item.Key, new Expirable<TValueType>(item.Value, LifeSpan));
        }

        public void Clear()
        {
            InternalDictionary.Clear();
        }

        public void Add(TKeyType key, TValueType value)
        {
            InternalDictionary.TryAdd(key, new Expirable<TValueType>(value, LifeSpan));
        }

        public bool ContainsKey(TKeyType key)
        {
            if (!InternalDictionary.TryGetValue(key, out var expirable))
            {
                return false;
            }

            if (!expirable.IsExpired)
            {
                return true;
            }

            InternalDictionary.TryRemove(key, out _);
            return false;
        }

        public bool Remove(TKeyType key)
        {
            return InternalDictionary.TryRemove(key, out var value);
        }

        public bool TryGetValue(TKeyType key, out TValueType value)
        {
            var found = false;
            if (InternalDictionary.TryGetValue(key, out var resolved))
            {
                if (!resolved.IsExpired)
                {
                    value = resolved.Value;
                    return true;
                }

                InternalDictionary.TryRemove(key, out _);
            }

            value = default(TValueType);
            return false;
        }
    }
}