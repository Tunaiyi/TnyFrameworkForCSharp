using System;
using System.IO;

namespace TnyFramework.Codec
{

    public abstract class ObjectCodec<T> : IObjectCodec<T>
    {
        public abstract byte[] Encode(T value);

        public abstract void Encode(T value, Stream output);

        public abstract T Decode(byte[] bytes);

        public abstract T Decode(Stream input);

        public abstract string Format(T value);

        public abstract T Parse(string data);

        public byte[] Encode(object value)
        {
            if (value is T data)
            {
                return Encode(data);
            } else
            {
                throw new InvalidCastException($"{value} 无法转位 {nameof(T)}");
            }
        }

        public void Encode(object value, Stream output)
        {
            if (value is T data)
            {
                Encode(data, output);
            } else
            {
                throw new InvalidCastException($"{value} 无法转位 {nameof(T)}");
            }
        }

        object IObjectCodec.Decode(byte[] bytes)
        {
            return Decode(bytes);
        }

        object IObjectCodec.Decode(Stream input)
        {
            return Decode(input);
        }

        public string Format(object value)
        {
            if (value is T data)
            {
                return Format(data);
            } else
            {
                throw new InvalidCastException($"{value} 无法转位 {nameof(T)}");
            }
        }

        object IObjectCodec.Parse(string data)
        {
            return Parse(data);
        }
    }

}
