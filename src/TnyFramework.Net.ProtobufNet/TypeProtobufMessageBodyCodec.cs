using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Common;
using Microsoft.Extensions.Logging;
using ProtoBuf;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;
using TnyFramework.Common.Logger;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.ProtobufNet
{

    public class TypeProtobufMessageBodyCodec : IMessageBodyCodec
    {
        private const int PROTOBUF_RAW_TYPE_BIT_SIZE = 5; //rawType 占据字节数
        private const byte PROTOBUF_RAW_TYPE_BIT_MASK = 0xFF >> (8 - PROTOBUF_RAW_TYPE_BIT_SIZE); //rawType 掩码 00011111
        private const byte PROTOBUF_MESSAGE_IS_PARAMS = 0x80; //是否是paramlist

        private readonly Dictionary<Type, DataCoder> typeCoders = new Dictionary<Type, DataCoder>();
        private readonly Dictionary<ProtobufRawType, DataCoder> rawTypeCoders = new Dictionary<ProtobufRawType, DataCoder>();

        private readonly NullCoder nullCoder;
        private readonly ComplexCoder complexCoder;

        public static readonly ILogger LOGGER = LogFactory.Logger<TypeProtobufMessageBodyCodec>();


        public TypeProtobufMessageBodyCodec()
        {
            nullCoder = new NullCoder();
            complexCoder = new ComplexCoder();
            Register(new ByteCoder());
            Register(new ShortCoder());
            Register(new IntCoder());
            Register(new LongCoder());
            Register(new FloatCoder());
            Register(new DoubleCoder());
            Register(new BoolCoder());
            Register(new StringCoder());
        }


        public void Encode(object body, IByteBuffer buffer)
        {
            switch (body)
            {
                case null:
                    return;
                case MessageParamList paramList: {
                    foreach (var param in paramList)
                    {
                        DoEncode(param, buffer, true);
                    }
                    break;
                }
                default:
                    DoEncode(body, buffer, false);
                    break;
            }
        }


        private void DoEncode(object body, IByteBuffer buffer, bool paramList)
        {
            DataCoder coder = nullCoder;
            if (body != null)
            {
                var type = body.GetType();
                if (!typeCoders.TryGetValue(type, out coder))
                {
                    coder = complexCoder;
                }
            }
            var option = (byte) (paramList ? PROTOBUF_MESSAGE_IS_PARAMS : 0);
            coder.Encode(body, buffer, option);
        }



        public object Decode(IByteBuffer buffer)
        {
            object body = null;
            IList listValue = null;
            while (buffer.ReadableBytes > 0)
            {
                var option = buffer.ReadByte();
                var paramList = (option & PROTOBUF_MESSAGE_IS_PARAMS) != 0;
                if (paramList && listValue == null)
                {
                    listValue = new MessageParamList();
                    body = listValue;
                }
                var value = DoDecode((byte) (option & PROTOBUF_RAW_TYPE_BIT_MASK), buffer);
                if (listValue != null)
                {
                    if (value != null)
                    {
                        listValue.Add(value);
                    }
                } else
                {
                    body = value;
                }
            }
            return body;
        }


        private object DoDecode(byte rawTypeValue, IByteBuffer buffer)
        {
            var rawType = (ProtobufRawType) rawTypeValue;
            DataCoder coder = nullCoder;
            if (rawType == ProtobufRawType.Null)
                return coder.Decode(buffer);
            if (!rawTypeCoders.TryGetValue(rawType, out coder))
            {
                coder = complexCoder;
            }
            return coder.Decode(buffer);
        }


        private void Register(DataCoder coder)
        {
            if (!rawTypeCoders.ContainsKey(coder.RawType))
            {
                rawTypeCoders.Add(coder.RawType, coder);
                foreach (var coderValueType in coder.ValueTypes)
                {
                    if (!typeCoders.ContainsKey(coderValueType))
                    {
                        typeCoders.Add(coderValueType, coder);
                    } else
                    {
                        throw new ArgumentException($"已存在该coder：{coderValueType}");
                    }
                }
            } else
            {
                throw new ArgumentException($"已存在该coder：{coder.RawType}");
            }
        }
    }


    internal abstract class DataCoder
    {
        private static readonly FastThreadLocal<MemoryStream> STEAM_LOCAL = new FastThreadLocal<MemoryStream>();


        protected static MemoryStream Stream(long length = -1)
        {
            var stream = STEAM_LOCAL.Value;
            if (stream == null)
            {
                stream = new MemoryStream();
                STEAM_LOCAL.Value = stream;
            } else
            {
                if (stream.Position != 0)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
            if (length > 0 && stream.Length < length)
            {
                stream.SetLength(length);
            }
            return stream;
        }


        protected DataCoder(ProtobufRawType rawType, params Type[] valueTypes)
        {
            RawType = rawType;
            ValueTypes = valueTypes;
        }


        /// <summary>
        /// protobuf 基础类型
        /// </summary>
        public ProtobufRawType RawType { get; }

        /// <summary>
        /// 解释的基础类型
        /// </summary>
        public Type[] ValueTypes { get; }


        /// <summary>
        /// encode
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <param name="option"></param>
        public void Encode(object value, IByteBuffer buffer, byte option)
        {
            var temp = option | (byte) RawType;
            buffer.WriteByte((byte) temp);
            DoEncode(value, buffer);
        }


        /// <summary>
        /// decode
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public object Decode(IByteBuffer buffer)
        {
            return DoDecode(buffer);
        }


        /// <summary>
        /// 子类实现
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        protected abstract void DoEncode(object value, IByteBuffer buffer);


        /// <summary>
        /// 子类实现
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected abstract object DoDecode(IByteBuffer buffer);
    }


    internal class NullCoder : DataCoder
    {
        public NullCoder() : base(ProtobufRawType.Null)
        {
        }


        protected override void DoEncode(object value, IByteBuffer buffer)
        {
        }


        protected override object DoDecode(IByteBuffer buffer) => null;
    }

    internal class ByteCoder : DataCoder
    {
        public ByteCoder() : base(ProtobufRawType.Byte, typeof(sbyte), typeof(byte))
        {
        }


        protected override void DoEncode(object value, IByteBuffer buffer)
        {
            switch (value)
            {
                case byte byteValue:
                    buffer.WriteByte(byteValue);
                    break;
                case sbyte byteValue:
                    buffer.WriteByte(byteValue);
                    break;
            }
        }


        protected override object DoDecode(IByteBuffer buffer) => (sbyte) buffer.ReadByte();
    }

    internal class ShortCoder : DataCoder
    {
        public ShortCoder() : base(ProtobufRawType.Short, typeof(short))
        {
        }


        protected override void DoEncode(object value, IByteBuffer buffer)
        {
            ByteBufferUtils.WriteVariant((int) value, buffer);
        }


        protected override object DoDecode(IByteBuffer buffer)
        {
            ByteBufferUtils.ReadVariant(buffer, out int value);
            return (short) value;
        }
    }

    internal class IntCoder : DataCoder
    {
        public IntCoder() : base(ProtobufRawType.Int, typeof(int))
        {
        }


        protected override void DoEncode(object value, IByteBuffer buffer)
        {
            ByteBufferUtils.WriteVariant((int) value, buffer);
        }


        protected override object DoDecode(IByteBuffer buffer)
        {
            ByteBufferUtils.ReadVariant(buffer, out int value);
            return value;
        }
    }

    internal class LongCoder : DataCoder
    {
        public LongCoder() : base(ProtobufRawType.Long, typeof(long))
        {
        }


        protected override void DoEncode(object value, IByteBuffer buffer)
        {
            ByteBufferUtils.WriteVariant((long) value, buffer);
        }


        protected override object DoDecode(IByteBuffer buffer)
        {
            ByteBufferUtils.ReadVariant(buffer, out long value);
            return value;
        }
    }

    internal class FloatCoder : DataCoder
    {
        public FloatCoder() : base(ProtobufRawType.Float, typeof(float))
        {
        }


        protected override void DoEncode(object value, IByteBuffer buffer)
        {
            ByteBufferUtils.WriteVariant((float) value, buffer);
        }


        protected override object DoDecode(IByteBuffer buffer)
        {
            ByteBufferUtils.ReadVariant(buffer, out float value);
            return value;
        }
    }

    internal class DoubleCoder : DataCoder
    {
        public DoubleCoder() : base(ProtobufRawType.Double, typeof(double))
        {
        }


        protected override void DoEncode(object value, IByteBuffer buffer)
        {
            ByteBufferUtils.WriteVariant((double) value, buffer);
        }


        protected override object DoDecode(IByteBuffer buffer)
        {
            ByteBufferUtils.ReadVariant(buffer, out double value);
            return value;
        }
    }

    internal class BoolCoder : DataCoder
    {
        public BoolCoder() : base(ProtobufRawType.Bool, typeof(bool))
        {
        }


        protected override void DoEncode(object value, IByteBuffer buffer) => buffer.WriteByte((byte) ((bool) value ? 1 : 0));

        protected override object DoDecode(IByteBuffer buffer) => buffer.ReadByte() > 0;
    }

    internal class StringCoder : DataCoder
    {
        public StringCoder() : base(ProtobufRawType.String, typeof(string))
        {
        }


        protected override void DoEncode(object value, IByteBuffer buffer)
        {
            switch (value)
            {
                case string strValue:
                    var stream = Stream();
                    var data = Encoding.UTF8.GetBytes(strValue);
                    stream.Write(data, 0, data.Length);
                    // Serializer.Serialize(stream, strValue);
                    var length = stream.Position;
                    if (length <= 0)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        break;
                    }
                    ByteBufferUtils.WriteVariant(length, buffer);
                    buffer.WriteBytes(stream.GetBuffer(), 0, (int) length);
                    stream.Seek(0, SeekOrigin.Begin);
                    break;
            }
        }


        protected override object DoDecode(IByteBuffer buffer)
        {
            ByteBufferUtils.ReadVariant(buffer, out int bodyLength);
            if (bodyLength == 0)
            {
                return "";
            }
            var stream = Stream(bodyLength);
            buffer.ReadBytes(stream, bodyLength);
            return Encoding.UTF8.GetString(stream.GetBuffer(), 0, bodyLength);
        }
    }



    internal class ComplexCoder : DataCoder
    {
        private readonly TypeProtobufObjectCodecFactory factory;


        public ComplexCoder() : base(ProtobufRawType.Complex, typeof(object))
        {
            factory = new TypeProtobufObjectCodecFactory();
        }


        protected override void DoEncode(object value, IByteBuffer buffer)
        {
            var type = value.GetType();
            var codec = factory.CreateCodec(type);
            if (codec == null)
                throw new Exception($"不存在该DTO:{type}");
            var stream = Stream();
            try
            {
                codec.Encode(value, stream);
                if (stream.Position <= 0)
                    return;
                ByteBufferUtils.WriteVariant(stream.Position, buffer);
                buffer.WriteBytes(stream.GetBuffer(), 0, (int) stream.Position);
            } finally
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
        }


        protected override object DoDecode(IByteBuffer buffer)
        {
            var codec = factory.CreateCodec(typeof(object));
            ByteBufferUtils.ReadVariant(buffer, out int bodyLength);
            var body = buffer.ReadBytes(bodyLength);
            using (var stream = new ReadOnlyByteBufferStream(body, true))
            {
                return codec.Decode(stream);
            }
        }
    }

}
