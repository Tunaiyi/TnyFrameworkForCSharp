using System.IO;

namespace TnyFramework.Codec
{

    public interface IObjectCodec
    {
        byte[] Encode(object value);

        void Encode(object value, Stream output);

        object Decode(byte[] bytes);

        object Decode(Stream input);

        string Format(object value);

        object Parse(string data);
    }

    public interface IObjectCodec<T> : IObjectCodec
    {
        byte[] Encode(T value);

        void Encode(T value, Stream output);

        new T Decode(byte[] bytes);

        new T Decode(Stream input);

        string Format(T value);

        new T Parse(string data);
    }

}
