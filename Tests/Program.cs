using System;
using System.Collections.Concurrent;
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

            Task.Run(async () =>
            {
                var dyn = new Dynamic<string>().Throttled(TimeSpan.FromSeconds(0.1));
                dyn.WithAsyncEvaluator(async () =>
                {
                    await Task.Delay(1000);
                    var result = RandomUtil.RandomString(50); 
                    return result;
                });

                for (var i = 0; i < 10; i++)
                {
                    Console.WriteLine(await dyn.GetValueAsync());
                }

                await Task.Delay(2000);

                for (var i = 0; i < 10; i++)
                {
                    Console.WriteLine(await dyn.GetValueAsync());
                }
            });
           

            Console.ReadLine();

            return;

            var lazyFactory = new LazyFactory<int, Dynamic<Task<ConcurrentDictionary<(Guid? t, Guid? t2), int>>>>()
                .SetInitializer(i =>
                {
                    return new Dynamic<Task<ConcurrentDictionary<(Guid? t, Guid? t2), int>>>()
                        .WithEvaluator(async () => new ConcurrentDictionary<(Guid? t, Guid? t2), int>())
                        .Throttled(TimeSpan.FromMinutes(20));
                });

            for (var i = 0; i < 5000; i++)
            {
                var i1 = i;
                _ = Task.Factory.StartNew(async () =>
                  {
                      for (var j = 0; j < 3; j++)
                      {
                          await Task.Delay(100);
                          var value = await lazyFactory[j].Value;
                          Console.WriteLine($"i {i1} j {j} " + value.GetType().Name);

                          if (RandomUtil.Range(0, 100) == 99)
                          {
                              lazyFactory[j].Poison();
                              Console.WriteLine("=======================            Poison " + j);
                          }
                      }

                      if (RandomUtil.Range(0, 100) <= 3)
                      {
                          lazyFactory.Clear();
                          Console.WriteLine("=======================            Clear ");
                      }
                  }, TaskCreationOptions.LongRunning);

            } 

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