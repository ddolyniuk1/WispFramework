using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WispFramework.Extensions;
using WispFramework.Patterns;
using WispFramework.Patterns.Containers;
using WispFramework.Patterns.Generators;
using WispFramework.Utility;

namespace WispFramework.UnitTests
{
    [TestClass]
    public class ExpirableCacheTests
    { 
        [TestMethod]
        public async Task TestRemove()
        {
            var k1 = Guid.NewGuid(); 
            var cache = new ExpirableCache<Guid, string>(TimeSpan.FromSeconds(1f)); 
            cache[k1] = "I will expire in 1 second"; 
            Assert.IsTrue(cache.TryGetValue(k1, out _));
            cache.Remove(k1);
            Assert.IsFalse(cache.TryGetValue(k1, out _));
        }

        public async Task TestDynamic()
        {
            // initialize a dynamic that generates a random string when Value is requested
            var dyn = new Dynamic<string>(() => RandomUtil.RandomString(1, 5));

            // will print a random string every time
            for (int i = 0; i < 10; i++)
            { 
                Console.WriteLine(dyn.Value);
            }

            // when throttled we use the cached data until expiry time
            dyn = new Dynamic<string>(() => RandomUtil.RandomString(1, 5))
                .Throttle(TimeSpan.FromSeconds(1));

            // will print the same value unless 1 second has elapsed
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(dyn.Value);
            } 
        }

        [TestMethod]
        public async Task TestTimeout()
        {
            var time = DateTime.Now;
            await Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                })
                .TimeoutAfter(TimeSpan.FromSeconds(1));
            Assert.IsTrue(DateTime.Now - time < TimeSpan.FromSeconds(2));
        }

        [TestMethod]
        public async Task TestContainsKey()
        {
            var k1 = Guid.NewGuid(); 
            var cache = new ExpirableCache<Guid, string>(TimeSpan.FromSeconds(1f));
            cache[k1] = "I will expire in 1 second"; 
            Assert.IsTrue(cache.ContainsKey(k1)); 
        }

        [DataTestMethod]
        [DataRow(10)]
        public async Task TestKeys(int count)
        { 
            var cache = new ExpirableCache<Guid, string>(TimeSpan.FromSeconds(1f));
            for (var i = 0; i < count; i++)
            {
                cache[Guid.NewGuid()] = "I will expire in 1 second";
            }  
            Assert.IsTrue(cache.Keys.Count == count);

            await Task.Delay(1000);

            Assert.IsTrue(cache.Keys.Count == 0);
        }


        [DataTestMethod]
        [DataRow(10)]
        public async Task TestValues(int count)
        { 
            var cache = new ExpirableCache<Guid, string>(TimeSpan.FromSeconds(1f));
            for (var i = 0; i < count; i++)
            {
                cache[Guid.NewGuid()] = "I will expire in 1 second";
            } 
            Assert.IsTrue(cache.Values.Count == count);

            await Task.Delay(1000);

            Assert.IsTrue(cache.Values.Count == 0);
        }

        [DataTestMethod]
        [DataRow(10)]
        public async Task TestIteration(int count)
        { 
            var cache = new ExpirableCache<Guid, string>(TimeSpan.FromSeconds(1f));
            for (var i = 0; i < count; i++)
            {
                cache[Guid.NewGuid()] = "I will expire in 1 second";
            }

            foreach (var item in cache)
            {
                cache.Remove(item.Key);
            }

            Assert.IsTrue(cache.Count == 0); 
        }


        [TestMethod]
        private async Task TestInitializer()
        {
            var k1 = Guid.NewGuid(); 
            var cache = new ExpirableCache<Guid, string>(TimeSpan.FromSeconds(1f))
            {
                {k1, "I will expire in 1 second"}
            }; 
            Assert.IsTrue(cache.TryGetValue(k1, out _));
            await Task.Delay(1000);
            Assert.IsFalse(cache.TryGetValue(k1, out _));
        }
    }
}
