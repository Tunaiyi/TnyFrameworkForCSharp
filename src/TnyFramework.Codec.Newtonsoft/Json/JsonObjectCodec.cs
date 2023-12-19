// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

#if NET
using System.Buffers;
#endif
using System.IO;
using System.Text;
using Newtonsoft.Json;
using TnyFramework.Common.Extensions;

namespace TnyFramework.Codec.Newtonsoft.Json
{

    public class JsonObjectCodec<T> : ObjectCodec<T>
    {
        private readonly JsonSerializerSettings? formatting;

        public JsonObjectCodec(JsonSerializerSettings? formatting)
        {
            this.formatting = formatting;
        }

        private string SerializeObject(T? value)
        {
            return formatting == null ? JsonConvert.SerializeObject(value) : JsonConvert.SerializeObject(value, formatting);
        }

        private T? DeserializeObject(string? json)
        {
            if (json == null)
            {
                return default;
            }
            return formatting == null ? JsonConvert.DeserializeObject<T>(json) : JsonConvert.DeserializeObject<T>(json, formatting);
        }

        public override byte[] Encode(T? value)
        {
            var json = SerializeObject(value);
            return Encoding.UTF8.GetBytes(json);
        }

        public override void Encode(T? value, Stream output)
        {
            var json = SerializeObject(value);
            var data = Encoding.UTF8.GetBytes(json);
            output.Write(data, 0, data.Length);
        }

        public override T? Decode(byte[]? bytes)
        {
            if (bytes == null)
            {
                return default;
            }
            var json = Encoding.UTF8.GetString(bytes);
            return DeserializeObject(json);
        }

        public override T? Decode(Stream input)
        {
            var reader = new StreamReader(input);
            var json = reader.ReadToEnd();
            return DeserializeObject(json);
        }

        public override string Format(T? value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public override T? Parse(string? data)
        {
            return DeserializeObject(data);
        }

#if NET
        public override void Encode(T? value, IBufferWriter<byte> output)
        {
            var json = SerializeObject(value);
            var data = Encoding.UTF8.GetBytes(json);
            output.Write(data);
        }

        public override T? Decode(ReadOnlySequence<byte> input)
        {
            if (input.IsNull() || input.Length == 0)
            {
                return default;
            }
            var json = Encoding.UTF8.GetString(input);
            return DeserializeObject(json);
        }
#endif
    }

}
