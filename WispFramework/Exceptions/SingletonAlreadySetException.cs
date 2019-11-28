using System;
using System.Collections.Generic;
using System.Text;

namespace WispFramework.Exceptions
{
    public class SingletonAlreadySetException : Exception
    {
        public override string Message => "This singleton object has already been initialized. Please use the Instance property instead.";
    }
}
