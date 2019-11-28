using System;
using WispFramework.EventArguments;

namespace WispFramework.Patterns.Observables
{
    [Serializable]
    public class Sub<T>
    {
        private T _value;

        public event EventHandler<ValueChangedEventArgs<T>> ValueChanged; 
         
        private readonly object _syncLock = new object();
         
        public Sub(T t)
        {
            Set(t);
        }

        public Sub()
        {
            _value = default(T);
        }
         
        public virtual T Value
        {
            get => _value;
            set
            {
                var oldValue = _value;
                if (Equals(_value, value))
                {
                    return;
                }

                Lock(() => { _value = value; });
                ValueChanged?.Invoke(this, new ValueChangedEventArgs<T>(oldValue, value));
            }
        }

        protected virtual void Lock(Action t)
        {
            lock (_syncLock)
            {
                t();
            }
        }

        public void Reset()
        {
            _value = default(T); 
        }
         
        public static Sub<T> Init(T t)
        {
            var obs = new Sub<T>();
            obs.Set(t);
            return obs;
        }

        public void InvokeChanged()
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgs<T>(default(T), _value));
        }
  
        public void Set(T value)
        {
            Value = value;
        }

        public static implicit operator T(Sub<T> v)
        {
            return v.Value;
        }

        public override string ToString()
        {
            return Value == null ? "null" : Value.ToString();
        }
    }
}
