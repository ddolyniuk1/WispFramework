namespace WispFramework.Patterns.Containers
{
    public abstract class ContainerSingleton<T> : Container
        where T : new()
    { 
        public static T Instance { get; } = new T();
    }
}
