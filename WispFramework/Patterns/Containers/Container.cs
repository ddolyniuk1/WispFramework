using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace WispFramework.Patterns.Containers
{
    /// <summary>
    /// Container stores objects by type or specific key
    /// </summary>
    public class Container
    {
        private ConcurrentDictionary<string, object> InternalContainer { get; } =
            new ConcurrentDictionary<string, object>();

        private static string Hash<T>()
        {
            return typeof(T).FullName;
        }

        private static string Hash(object obj)
        {
            return obj.GetType().FullName;
        }

        public void Clear()
        {
            InternalContainer.Clear();
        }

        public string FindKeyByValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            var result = InternalContainer.FirstOrDefault(t => t.Value == value);
            return !result.Equals(new KeyValuePair<string, object>()) ? result.Key : null;
        }

        public void Register(object o, string name)
        {
            InternalContainer[name] = o;
        }

        public void Register(object o)
        {
            Register(o, Hash(o));
        }

        public void Register<T>(T o)
        {
            Register(o, Hash<T>());
        }

        public T Resolve<T>()
        {
            return Resolve<T>(Hash<T>());
        }

        public T Resolve<T>(string name)
        {
            if (!InternalContainer.TryGetValue(name, out var value))
            {
                return default;
            }

            if (value is T result)
            {
                return result;
            }

            return default;
        }

        public List<T> ResolveMany<T>()
        {
            return InternalContainer.Values.OfType<T>().ToList();
        }

        public bool TryRemove<T>(T obj)
        {
            if (obj == null)
            {
                return false;
            }

            var key = Hash(obj);
            return InternalContainer.TryRemove(key, out _);
        }

        public bool TryRemove<T>(string key)
        {
            return InternalContainer.TryRemove(key, out _);
        }
    }
}