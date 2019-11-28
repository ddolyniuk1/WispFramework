using System.Linq;
using System.Security.Cryptography;

namespace WispFramework.Utility
{
    public class RandomUtil
    {
        private static readonly RNGCryptoServiceProvider RngCrypto = new RNGCryptoServiceProvider();
        private static readonly System.Random RandomObject = new System.Random();
        private static readonly object SyncLock = new object();
        
        public static int Range(int min, int max)
        {
            lock (SyncLock)
            { 
                return RandomObject.Next(min, max);
            }
        }

        public static byte[] CryptoRandomBytes(int count)
        {
            lock (SyncLock)
            {
                var b = new byte[count];
                RngCrypto.GetBytes(b);
                return b;
            }
        }
        
        public static string RandomString(int length)
        {
            lock (SyncLock)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[RandomObject.Next(s.Length)]).ToArray());
            }
        }

        public static string RandomString(int minLength, int maxLength)
        {
            return RandomString(Range(minLength, maxLength));
        }
    }
}
