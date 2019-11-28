using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WispFramework.EventArguments;
using WispFramework.Extensions;
using WispFramework.Patterns;
using WispFramework.Patterns.Observables;
using WispFramework.Utility;

namespace Tests
{
    public class EventAwaiterTests
    {  
        
        public static async void Run()
        {
            Sub<bool> hasCoffee = new Sub<bool>(false); 
            Task.Run(() => NewMethod(hasCoffee));
            
            string input;
            while ((input = Console.ReadLine()) != "exit")
            {
                if (input == "give coffee")
                {
                    hasCoffee.Value = true;
                }
            }
        }

        private static async Task NewMethod(Sub<bool> hasCoffee)
        {
            var waitForCoffee = new EventAwaiter<ValueChangedEventArgs<bool>>(
                h => hasCoffee.ValueChanged += h,
                h => hasCoffee.ValueChanged -= h);

            await waitForCoffee.Task;

            try
            {
                Console.WriteLine($"Value changed for hasCoffee to {hasCoffee}");
            }
            catch (TimeoutException)
            {
                Console.WriteLine("We did not get coffee in time!");
            }
        }
    }
}
