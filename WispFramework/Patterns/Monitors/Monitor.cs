using System;
using System.Threading;
using System.Threading.Tasks;
using WispFramework.EventArguments;

namespace WispFramework.Patterns.Monitors
{
    /// <summary>
    /// This class will evaluate the Condition on an Interval until Timeout is reached or cancel occurs
    /// </summary>
    public class Monitor
    {
        public Func<bool> Condition { get; set; }

        public TimeSpan Interval { get; set; }

        public TimeSpan Timeout { get; set; }

        public CancellationToken CancellationToken { get; set; }

        public bool Running { get; set; }

        public DateTime StartTime { get; set; }

        public event EventHandler<MonitorEventArgs> Completed;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="interval">How long we wait before trying the condition again</param>
        /// <param name="timeout">How long until we stop monitoring the condition - default to 10 seconds.</param>
        /// <param name="cancellationToken"></param>
        public Monitor(Func<bool> condition, TimeSpan interval, TimeSpan? timeout, CancellationToken? cancellationToken)
        {
            Condition = condition;
            Interval = interval;
            Timeout = timeout ?? TimeSpan.FromSeconds(10);
            
            CancellationToken = cancellationToken ?? CancellationToken.None;
        }

        public async void Start()
        {
            var result = await Task.Run(StartAsync, CancellationToken);
            Completed?.Invoke(this, new MonitorEventArgs(result));
        }
        
        private async Task<bool> StartAsync()
        {
            try
            {
                Running = true;
                StartTime = DateTime.Now;

                while (Running)
                {
                    if (Condition())
                    {
                        return true;
                    }

                    if (DateTime.Now - StartTime > Timeout)
                    {
                        return false;
                    }
                    await Task.Delay(Interval, CancellationToken);
                }

                return false;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
    }
}
