using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WispFramework.Extensions;
using WispFramework.Patterns;
using WispFramework.Patterns.Generators;
using WispFramework.Utility;

namespace Tests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //EventAwaiterTests.Run(); 
  
            var lazyFactory = new LazyFactory<int, Dynamic<string>>()
                .SetInitializer(i =>
                {
                    return new Dynamic<string>()
                        .SetEvaluator(() => $"Dyn {i}: " + " " +
                                            RandomUtil.RandomString(RandomUtil.Range(1,
                                                10)))
                        .Throttle(TimeSpan.FromSeconds(5));
                });

            Task.Run(async () =>
            {
                while (true)
                {
                    for (var i = 0; i < 3; i++)
                    { 
                        await Task.Delay(100);
                        Console.WriteLine(lazyFactory[i].Value);
                    }
                }
            });


            Console.ReadLine();
        }
         
        private static async void NewMethod(Dynamic<string> dyn)
        {
            while (true)
            {
                await Task.Delay(100);
                Console.WriteLine(dyn.Value);
            }
        }
    }
}