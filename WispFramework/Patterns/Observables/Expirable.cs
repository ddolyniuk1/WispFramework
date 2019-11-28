using System;
using WispFramework.EventArguments;

namespace WispFramework.Patterns.Observables
{
    public class Expirable<T> : Sub<T>
    {
        private readonly Sub<bool> _isExpired = new Sub<bool>(false);

        public Expirable(T t, TimeSpan lifespan)
        {
            Set(t);
            LifeSpan = lifespan;
            CreationDate = DateTime.Now;

            _isExpired.ValueChanged += IsExpiredOnValueChanged;
        }

        public DateTime CreationDate { get; set; }

        public TimeSpan LifeSpan { get; set; }

        public override T Value
        {
            get
            {
                CheckExpiration();  
                return base.Value;
            }
            set => base.Value = value;
        }

        private void CheckExpiration()
        {
            var expired = DateTime.Now - CreationDate > LifeSpan;
            if (expired)
            {
                _isExpired.Set(true);
            }
        }

        public bool IsExpired
        {
            get
            {
                CheckExpiration();
                return _isExpired.Value;
            }
        }

        public event EventHandler Expired;
        public event EventHandler Revived;

        private void IsExpiredOnValueChanged(object sender, ValueChangedEventArgs<bool> e)
        {
            if (e.NewValue)
            {
                Expired?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Revived?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Revive()
        {
            _isExpired.Set(false);
        }

        public static implicit operator T(Expirable<T> v)
        {
            return v.Value;
        }

        public override string ToString()
        {
            return Value == null ? "null" : Value.ToString();
        }
    }
}