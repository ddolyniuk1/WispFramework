﻿using System;
using System.Data;
using WispFramework.EventArguments;

namespace WispFramework.Patterns.Generators
{
    /// <summary>
    ///    Evaluates the expression whenever Value is requested
    ///    When throttling, we evaluate only when the data is considered stale
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Dynamic<T>
    {
        public EventHandler<ValueChangedEventArgs<T>> ValueUpdated;
        private bool _isSet;
        private T _t;
        private readonly object _syncLock = new object();

        public Dynamic(Func<T> evaluator)
        {
            Evaluator = evaluator;
            LastUpdateTime = DateTime.Now;
        }

        public Dynamic()
        { 
        }

        private Func<T> Evaluator { get; set; }

        private DateTime LastUpdateTime { get; set; }
        private TimeSpan? ExpiryTime { get; set; }
        
        public Dynamic<T> SetEvaluator(Func<T> evaluator)
        {
            Evaluator = evaluator;
            return this;
        }


        /// <summary>
        /// Prevents evaluation of the data until it is considered stale
        /// which is on or after LastUpdateTime + ExpiryTime
        /// </summary>
        /// <param name="expiryTime"></param>
        public Dynamic<T> Throttle(TimeSpan expiryTime)
        {
            ExpiryTime = expiryTime;
            return this;
        }

        /// <summary>
        /// Allows evaluation to occur on every request
        /// </summary>
        public Dynamic<T> UnThrottle()
        {
            ExpiryTime = null;
            return this;
        }

        private bool _isPoisoned;

        public void Poison()
        {
            lock(_syncLock)
            { 
                _isPoisoned = true;
            }
        }

        public T Value
        {
            get
            {
                lock (_syncLock)
                {
                    if (ExpiryTime.HasValue)
                    {
                        if (_isSet && DateTime.Now - LastUpdateTime <= ExpiryTime && !_isPoisoned)
                        {
                            return _t;
                        }
                    }

                    if (Evaluator == null)
                    {
                        throw new EvaluateException(
                            "Evaluator is not set, please set the Evaluator property before requesting a Value.");
                    }

                    Set(Evaluator.Invoke());
                    return _t;
                }
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
         
        public T Get()
        {
            lock (_syncLock)
            {
                return _t;
            }
        }

        public static implicit operator T(Dynamic<T> v)
        {
            return v.Value;
        }

        public override string ToString()
        {
            return Value == null ? "null" : Value.ToString();
        }
    }
}