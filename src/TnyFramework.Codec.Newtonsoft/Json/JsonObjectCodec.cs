using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TnyFramework.Codec.Newtonsoft.Json
{

    public class JsonObjectCodec<T> : ObjectCodec<T>
    {
        public override byte[] Encode(T value)
        {
            var json = JsonConvert.SerializeObject(value);
            return Encoding.UTF8.GetBytes(json);
        }

        public override void Encode(T value, Stream output)
        {
            var json = JsonConvert.SerializeObject(value);
            var data = Encoding.UTF8.GetBytes(json);
            output.Write(data, 0, data.Length);
        }

        public override T Decode(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public override T Decode(Stream input)
        {
            var reader = new StreamReader(input);
            var json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public override string Format(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public override T Parse(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }

}
