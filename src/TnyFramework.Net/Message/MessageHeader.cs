namespace TnyFramework.Net.Message
{

    public abstract class MessageHeader
    {
        public abstract string Key { get; }

        public object GetValue()
        {
            return this;
        }
    }

    /// <summary>
    /// 消息头信息
    /// </summary>
    public abstract class MessageHeader<T> : MessageHeader where T : MessageHeader<T>
    {
        public T Value => (T) this;
    }

}
