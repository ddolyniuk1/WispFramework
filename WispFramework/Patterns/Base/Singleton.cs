using WispFramework.Exceptions;

namespace WispFramework.Patterns.Base
{ 
    public abstract class Singleton<T> where T : new()
    {
        public static T Instance { get; } = new T();

        protected Singleton()
        {
            if (Instance != null)
            {
                throw new SingletonAlreadySetException();
            }
        }
    } 
}
