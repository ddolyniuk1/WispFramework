namespace WispFramework.EventArguments
{
    public class ElementStateChangedEventArgs<TKeyType, TValueType>
    {
        public TKeyType Key { get; set; }
        public TValueType Value { get; set; }

        public ElementStateChangedEventArgs(TKeyType key, TValueType value)
        {
            Key = key;
            Value = value;
        }
    }
}
