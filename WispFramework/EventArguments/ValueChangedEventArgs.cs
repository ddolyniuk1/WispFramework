using System;

namespace WispFramework.EventArguments
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public T NewValue { get; set; }
        public T OldValue { get; set; }
    }
}
