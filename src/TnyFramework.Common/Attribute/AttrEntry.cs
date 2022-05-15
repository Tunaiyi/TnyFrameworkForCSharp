namespace TnyFramework.Common.Attribute
{

    public interface IAttrPair
    {
        IAttrKey Key { get; }

        object Value { get; }
    }

    public sealed class AttrPair<T> : IAttrPair
    {
        public AttrPair<T> Entry(ref AttrKey<T> key, T value)
        {
            return new AttrPair<T>(ref key, value);
        }

        private AttrPair(ref AttrKey<T> key, T value)
        {
            Key = key;
            Value = value;
        }

        public AttrKey<T> Key { get; }

        public T Value { get; }

        IAttrKey IAttrPair.Key => Key;

        object IAttrPair.Value => Value;
    }

}
