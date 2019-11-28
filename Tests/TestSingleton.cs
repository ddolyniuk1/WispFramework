using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using WispFramework.Patterns;
using WispFramework.Patterns.Base;

namespace Tests
{
    public class TestSingleton : Singleton<TestSingleton>
    {
        public TestSingleton()
        {
            Console.WriteLine("I was created!");
        }

        public void Emit(string message)
        {
            Console.WriteLine(message);
        }
    }
}
