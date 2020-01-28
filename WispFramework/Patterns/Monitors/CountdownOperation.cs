using System;
using System.Threading.Tasks;

namespace WispFramework.Patterns.Monitors
{
    public class CountdownOperation
    {
        private readonly object _syncLock = new object();
        private bool _completed;

        private DateTime _lastUpdate = DateTime.MinValue;
        private Task WorkerTask = null;
        public CountdownOperation()
        {
        }

        /// <summary>
        ///     Invoke the CompletedCallback once the countdown completes
        /// </summary>
        /// <param name="startCallback"></param>
        /// <param name="completedCallback"></param>
        /// <param name="delay"></param>
        public CountdownOperation(Action startCallback, Action completedCallback, TimeSpan delay)
        {
            StartCallback = startCallback;
            CompletedCallback = completedCallback;
            Delay = delay;
            WorkLoop();
        }

        /// <summary>
        ///     Invoke the CompletedCallback once the countdown completes
        /// </summary>
        /// <param name="completedCallback"></param>
        /// <param name="delay"></param>
        public CountdownOperation(Action completedCallback, TimeSpan delay)
        {
            CompletedCallback = completedCallback;
            Delay = delay;
            WorkLoop();
        }

        private Action CompletedCallback { get; set; }
        private TimeSpan? Delay { get; set; }
        private Action StartCallback { get; set; }

        /// <summary>
        ///     Safely resets the start delay to Now
        /// </summary>
        public void Reset()
        {
            SetStartTime(DateTime.Now);
        }

        /// <summary>
        ///     Restarts the countdown
        /// </summary>
        public void Restart()
        {
            if (!_completed && WorkerTask != null)
            {
                Reset();
                return;
            }

            _completed = false;
            WorkLoop();
        }

        public CountdownOperation Begin()
        {
            Restart();
            return this;
        }

        public CountdownOperation WithCompletedCallback(Action completedCallback)
        {
            CompletedCallback = completedCallback;
            return this;
        }

        public CountdownOperation WithDelay(TimeSpan delay)
        {
            Delay = delay;
            return this;
        }

        public CountdownOperation WithStartCallback(Action startCallback)
        {
            StartCallback = startCallback;
            return this;
        }

        private DateTime GetStartTime()
        {
            lock (_syncLock)
            {
                return _lastUpdate;
            }
        }

        private void SetStartTime(DateTime time)
        {
            lock (_syncLock)
            {
                _lastUpdate = time;
            }
        }

        private void WorkLoop()
        {
            SetStartTime(DateTime.Now);
            if (Delay == null)
            {
                return;
            }

            WorkerTask = Task.Run(async () =>
            {
                StartCallback?.Invoke();

                while (DateTime.Now - GetStartTime() < Delay)
                {
                    await Task.Delay(50);
                }

                CompletedCallback?.Invoke();

                _completed = true;
            });
        }
    }
}