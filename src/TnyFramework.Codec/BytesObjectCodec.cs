using Microsoft.IdentityModel.Tokens;

namespace TnyFramework.Codec
{

    public abstract class BytesObjectCodec<T> : ObjectCodec<T>
    {
        public override string Format(T value)
        {
            if (value == null)
            {
                return null;
            }
            var bytes = Encode(value);
            return Base64UrlEncoder.Encode(bytes);
        }

        public override T Parse(string data)
        {
            if (data == null)
            {
                return default;
            }

            var bytes = Base64UrlEncoder.DecodeBytes(data);
            return Decode(bytes);
        }
    }

}
