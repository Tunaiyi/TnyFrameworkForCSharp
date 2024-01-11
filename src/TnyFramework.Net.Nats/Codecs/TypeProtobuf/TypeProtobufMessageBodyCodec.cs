// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;
using TnyFramework.Common.Binary.Extensions;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Nats.Codecs.TypeProtobuf
{

    public class TypeProtobufMessageBodyCodec : IMessageBodyCodec
    {
        private const int PROTOBUF_RAW_TYPE_BIT_SIZE = 5; //rawType 占据字节数
        private const byte PROTOBUF_RAW_TYPE_BIT_MASK = 0xFF >> (8 - PROTOBUF_RAW_TYPE_BIT_SIZE); //rawType 掩码 00011111
        private const byte PROTOBUF_MESSAGE_IS_PARAMS = 0x80; //是否是paramlist

        private readonly Dictionary<Type, DataCoder> typeCoders = new();

        private readonly Dictionary<ProtobufRawType, DataCoder> rawTypeCoders =
            new Dictionary<ProtobufRawType, DataCoder>();

        private readonly NullCoder nullCoder;
        private readonly ComplexCoder complexCoder;

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

        public void Encode(object? body, IBufferWriter<byte> buffer)
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

        private void DoEncode(object? body, IBufferWriter<byte> writer, bool paramList)
        {
            DataCoder coder;
            if (body != null)
            {
                var type = body.GetType();
                coder = typeCoders.GetValueOrDefault(type, complexCoder);
            } else
            {
                coder = nullCoder;
            }
            var option = (byte) (paramList ? PROTOBUF_MESSAGE_IS_PARAMS : 0);
            coder.Encode(body, writer, option);
        }

        public object? Decode(ReadOnlySequence<byte> buffer)
        {
            object? body = null;
            IList? listValue = null;
            var reader = new SequenceReader<byte>(buffer);
            while (reader.Remaining > 0)
            {
                reader.TryRead(out var option);
                var paramList = (option & PROTOBUF_MESSAGE_IS_PARAMS) != 0;
                if (paramList && listValue == null)
                {
                    listValue = new MessageParamList();
                    body = listValue;
                }
                var rawType = (ProtobufRawType) (option & PROTOBUF_RAW_TYPE_BIT_MASK);
                var value = DoDecode(rawType, ref reader);
                if (listValue != null)
                {
                    listValue.Add(value);
                } else
                {
                    body = value;
                }
            }
            return body;
        }

        private object? DoDecode(ProtobufRawType rawType, ref SequenceReader<byte> reader)
        {
            DataCoder coder = nullCoder;
            if (rawType == ProtobufRawType.Null)
                return coder.Decode(ref reader);
            coder = rawTypeCoders.GetValueOrDefault(rawType, complexCoder);
            return coder.Decode(ref reader);
        }

        private void Register(DataCoder coder)
        {
            if (rawTypeCoders.TryAdd(coder.RawType, coder))
            {
                foreach (var coderValueType in coder.ValueTypes)
                {
                    if (!typeCoders.TryAdd(coderValueType, coder))
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
        protected const int LENGTH_SIZE = sizeof(int);

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
        public void Encode(object? value, IBufferWriter<byte> buffer, byte option)
        {
            var temp = option | (byte) RawType;
            buffer.WriteByte((byte) temp);
            DoEncode(value, buffer);
        }

        /// <summary>
        /// decode
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public object? Decode(ref SequenceReader<byte> reader)
        {
            return DoDecode(ref reader);
        }

        /// <summary>
        /// 子类实现
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        protected abstract void DoEncode(object? value, IBufferWriter<byte> buffer);

        /// <summary>
        /// 子类实现
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected abstract object? DoDecode(ref SequenceReader<byte> reader);
    }

    internal class NullCoder : DataCoder
    {
        public NullCoder() : base(ProtobufRawType.Null)
        {
        }

        protected override void DoEncode(object? value, IBufferWriter<byte> buffer)
        {
        }

        protected override object? DoDecode(ref SequenceReader<byte> buffer) => null;
    }

    internal class ByteCoder : DataCoder
    {
        public ByteCoder() : base(ProtobufRawType.Byte, typeof(sbyte), typeof(byte))
        {
        }

        protected override void DoEncode(object? value, IBufferWriter<byte> buffer)
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

        protected override object? DoDecode(ref SequenceReader<byte> reader)
        {
            reader.TryRead(out var value);
            return (sbyte) value;
        }
    }

    internal class ShortCoder : DataCoder
    {
        public ShortCoder() : base(ProtobufRawType.Short, typeof(short))
        {
        }

        protected override void DoEncode(object? value, IBufferWriter<byte> buffer)
        {
            if (value is short v)
            {
                buffer.WriteVariant(v);
            }
        }

        protected override object DoDecode(ref SequenceReader<byte> reader)
        {
            reader.ReadVariant(out int value);
            return (short) value;
        }
    }

    internal class IntCoder : DataCoder
    {
        public IntCoder() : base(ProtobufRawType.Int, typeof(int))
        {
        }

        protected override void DoEncode(object? value, IBufferWriter<byte> buffer)
        {
            if (value is int v)
            {
                buffer.WriteVariant(v);
            }
        }

        protected override object DoDecode(ref SequenceReader<byte> reader)
        {
            reader.ReadVariant(out int value);
            return value;
        }
    }

    internal class LongCoder : DataCoder
    {
        public LongCoder() : base(ProtobufRawType.Long, typeof(long))
        {
        }

        protected override void DoEncode(object? value, IBufferWriter<byte> buffer)
        {
            if (value is long v)
            {
                buffer.WriteVariant(v);
            }
        }

        protected override object DoDecode(ref SequenceReader<byte> reader)
        {
            reader.ReadVariant(out long value);
            return value;
        }
    }

    internal class FloatCoder : DataCoder
    {
        public FloatCoder() : base(ProtobufRawType.Float, typeof(float))
        {
        }

        protected override void DoEncode(object? value, IBufferWriter<byte> buffer)
        {
            if (value is float v)
            {
                buffer.WriteFixed32(v);
            }
        }

        protected override object DoDecode(ref SequenceReader<byte> reader)
        {
            reader.ReadVariant(out float value);
            return value;
        }
    }

    internal class DoubleCoder : DataCoder
    {
        public DoubleCoder() : base(ProtobufRawType.Double, typeof(double))
        {
        }

        protected override void DoEncode(object? value, IBufferWriter<byte> buffer)
        {

            if (value is double v)
            {
                buffer.WriteFixed64(v);
            }
        }

        protected override object DoDecode(ref SequenceReader<byte> reader)
        {
            reader.ReadVariant(out double value);
            return value;
        }
    }

    internal class BoolCoder : DataCoder
    {
        public BoolCoder() : base(ProtobufRawType.Bool, typeof(bool))
        {
        }

        protected override void DoEncode(object? value, IBufferWriter<byte> buffer)
        {
            if (value is bool v)
            {
                buffer.WriteByte((byte) (v ? 1 : 0));
            }
        }

        protected override object? DoDecode(ref SequenceReader<byte> reader) => reader.ReadByte() > 0;
    }

    internal class StringCoder : DataCoder
    {
        public StringCoder() : base(ProtobufRawType.String, typeof(string))
        {
        }

        protected override void DoEncode(object? value, IBufferWriter<byte> buffer)
        {
            switch (value)
            {
                case string strValue: {
                    var lengthSpan = buffer.GetSpan(LENGTH_SIZE);
                    buffer.Advance(LENGTH_SIZE);
                    var size = buffer.WriteString(strValue);
                    lengthSpan.WriteInt32LittleEndian(size);
                    break;
                }
            }
        }

        protected override object DoDecode(ref SequenceReader<byte> reader)
        {
            var bodyLength = reader.ReadInt32LittleEndian();
            return bodyLength == 0 ? "" : reader.ReadString(bodyLength);
        }
    }

    internal class ComplexCoder : DataCoder
    {
        private readonly TypeProtobufObjectCodecFactory factory;


        public ComplexCoder() : base(ProtobufRawType.Complex, typeof(object))
        {
            factory = new TypeProtobufObjectCodecFactory();
        }

        protected override void DoEncode(object? value, IBufferWriter<byte> buffer)
        {
            var type = value?.GetType();
            var codec = type == null ? null : factory.CreateCodec(type);
            if (codec == null)
                throw new Exception($"不存在该DTO:{type}");
            var lengthSpan = buffer.GetSpan(LENGTH_SIZE);
            buffer.Advance(LENGTH_SIZE);
            var write = buffer.AsByteBufferWriter();
            codec.Encode(value, write);
            var length = write.Length;
            lengthSpan.WriteInt32LittleEndian(length);
        }

        protected override object? DoDecode(ref SequenceReader<byte> buffer)
        {
            var codec = factory.CreateCodec(typeof(object));
            var bodyLength = buffer.ReadInt32LittleEndian();
            var bodyBuffer = buffer.UnreadSequence.Slice(0, bodyLength);
            buffer.Advance(bodyLength);
            return codec.Decode(bodyBuffer);
        }

    }

}
