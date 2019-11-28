using System;
using System.Collections.Generic;
using System.Text;

namespace WispFramework.EventArguments
{
    public class MonitorEventArgs : EventArgs
    {
        public bool Success { get; set; }

        public MonitorEventArgs(bool success)
        {
            Success = success;
        }
    }
}
