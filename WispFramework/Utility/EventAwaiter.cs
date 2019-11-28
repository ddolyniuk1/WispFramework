using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WispFramework.Utility
{
    public class EventAwaiter<TEventArgs>
    {
        private readonly TaskCompletionSource<TEventArgs> _eventArrived = new TaskCompletionSource<TEventArgs>();

        private readonly Action<EventHandler<TEventArgs>> _unsubscribe;
        private readonly Func<object, TEventArgs, bool> _constraint;

        /// <summary>
        /// Example Usage:
        /// var waiter = new EventAwaiter<ValueChangedEventArgs<bool>>(
        /// h => SomeValue.ValueUpdated += h, 
        /// h => SomeValue.ValueUpdated -= h,
        /// (o, args) => args.NewValue); [Optional]
        /// </summary>
        /// <param name="subscribe"></param>
        /// <param name="unsubscribe"></param>
        /// <param name="constraint" optional="true"></param>
        public EventAwaiter(Action<EventHandler<TEventArgs>> subscribe, Action<EventHandler<TEventArgs>> unsubscribe, Func<object, TEventArgs, bool> constraint = null)
        {
            subscribe(Subscription);
            _unsubscribe = unsubscribe;
            _constraint = constraint;
        }

        public Task<TEventArgs> Task => _eventArrived.Task;

        private EventHandler<TEventArgs> Subscription => (s, e) =>
        {
            if (_constraint != null && !_constraint(s, e))
            {
                return;
            }
            _eventArrived.TrySetResult(e);
            _unsubscribe(Subscription);
        };
    }
}
