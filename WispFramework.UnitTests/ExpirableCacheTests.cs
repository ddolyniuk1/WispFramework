using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WispFramework.Patterns;
using WispFramework.Patterns.Containers;
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
