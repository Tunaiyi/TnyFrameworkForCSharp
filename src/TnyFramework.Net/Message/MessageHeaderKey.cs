using System;

namespace TnyFramework.Net.Message
{

    public abstract class MessageHeaderKey
    {
        public string Key { get; }

        public Type HeaderType { get; }


        protected MessageHeaderKey(string key, Type headerType)
        {
            Key = key;
            HeaderType = headerType;
        }


        private bool Equals(MessageHeaderKey other)
        {
            return Key == other.Key && HeaderType == other.HeaderType;
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((MessageHeaderKey) obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0) * 397) ^ (HeaderType != null ? HeaderType.GetHashCode() : 0);
            }
        }
    }

    /// <summary>
    /// 消息头信息 Key
    /// </summary>
    /// <typeparam name="TH"></typeparam>
    public class MessageHeaderKey<TH> : MessageHeaderKey where TH : MessageHeader<TH>
    {
        public static MessageHeaderKey<TH> Of(string key)
        {
            return new MessageHeaderKey<TH>(key);
        }


        private MessageHeaderKey(string key) : base(key, typeof(TH))
        {
        }
    }

}
