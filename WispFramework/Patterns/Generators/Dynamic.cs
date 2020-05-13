using System;
using System.Data;
using System.Threading.Tasks;
using WispFramework.EventArguments;
using WispFramework.Interfaces;

namespace WispFramework.Patterns.Generators
{
    /// <summary>
    ///     Evaluates the expression whenever Value is requested
    ///     When throttling, we evaluate only when the data is considered stale
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Dynamic<T> : IKeyValueObject
    {
        public EventHandler<ValueChangedEventArgs<T>> ValueUpdated;
        private readonly object _syncLock = new object();

        private bool _isPoisoned;
        private bool _isSet; 
        private T _t; 

        public Dynamic(Func<T> evaluator)
        {
            Evaluator = evaluator;
            LastUpdateTime = DateTime.Now;
        }
         
        public Dynamic()
        {
        }

        public T Value => GetValue();

        /// <summary>
        /// Updates the value if the data has expired, and returns the current value
        /// </summary>
        /// <returns></returns>
        public T GetValue()
        {
            if (ExpiryTime.HasValue)
            {
                if (_isSet && DateTime.Now - LastUpdateTime <= ExpiryTime && !_isPoisoned)
                {
                    return _t;
                }
            }

            Evaluate().GetAwaiter().GetResult();
            return _t;
        }

        /// <summary>
        /// Updates the value if the data has expired, and returns the current value
        /// </summary>
        /// <returns></returns>
        public async Task<T> GetValueAsync()
        {
            if (ExpiryTime.HasValue)
            {
                if (_isSet && DateTime.Now - LastUpdateTime <= ExpiryTime && !_isPoisoned)
                {
                    return _t;
                }
            }

            await Evaluate();
            return _t;
        }

        private async Task Evaluate()
        {
            if (Evaluator == null && AsyncEvaluator == null)
            {
                throw new EvaluateException(
                    "Evaluator is not set, please set the Evaluator property before requesting a Value.");
            }

            if (Evaluator != null)
            {
                Set(Evaluator()); 
            }
            else if (AsyncEvaluator != null)
            {
                Set(await AsyncEvaluator());
            }
        }

        private Func<T> Evaluator { get; set; }

        private Func<Task<T>> AsyncEvaluator { get; set; }

        private TimeSpan? ExpiryTime { get; set; }

        private DateTime LastUpdateTime { get; set; }

        public static implicit operator T(Dynamic<T> v)
        {
            return v.Value;
        }

        /// <summary>
        /// Gets the cached value
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            lock (_syncLock)
            {
                return _t;
            }
        }

        public void Poison()
        {
            lock (_syncLock)
            {
                _isPoisoned = true;
            }
        }

        public void Set(T value)
        {
             
            T oldValue = default;
            lock (_syncLock)
            {
                oldValue = _t;

                _isSet = true;
                _t = value;
                _isPoisoned = false;

                LastUpdateTime = DateTime.Now;
            }

            ValueUpdated?.Invoke(this, new ValueChangedEventArgs<T>(oldValue, value));
        }

        public Dynamic<T> WithEvaluator(Func<T> evaluator)
        {
            Evaluator = evaluator;
            return this;
        }

        public Dynamic<T> WithAsyncEvaluator(Func<Task<T>> evaluator)
        {
            AsyncEvaluator = evaluator;
            return this;
        }


        /// <summary>
        ///     Prevents evaluation of the data until it is considered stale
        ///     which is on or after LastUpdateTime + ExpiryTime
        /// </summary>
        /// <param name="expiryTime"></param>
        public Dynamic<T> Throttled(TimeSpan expiryTime)
        {
            ExpiryTime = expiryTime;
            return this;
        }

        public override string ToString()
        {
            return Value == null ? "null" : Value.ToString();
        }

        /// <summary>
        ///     Allows evaluation to occur on every request
        /// </summary>
        public Dynamic<T> UnThrottled()
        {
            ExpiryTime = null;
            return this;
        }
 
        public object Key { get; set; }
    }
}