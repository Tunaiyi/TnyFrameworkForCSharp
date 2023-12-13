using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace TnyFramework.Common.Binary.Extensions
{

    public static class SequenceReaderBinaryPrimitivesExtension
    {

        public static byte ReadByte(this ref SequenceReader<byte> reader)
        {
            if (reader.TryRead(out var value))
            {
                return value;
            }
            throw new IndexOutOfRangeException();
        }

        public static sbyte ReadSByte(this ref SequenceReader<byte> reader)
        {
            if (reader.TryRead(out var value))
            {
                return (sbyte) value;
            }
            throw new IndexOutOfRangeException();
        }

        public static double ReadDoubleLittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(double)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return span.ReadDoubleLittleEndian();
        }

        public static short ReadInt16LittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(short)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadInt16LittleEndian(span);
        }

        public static int ReadInt32LittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(int)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadInt32LittleEndian(span);
        }

        public static long ReadInt64LittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(long)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadInt64LittleEndian(span);
        }

        public static float ReadSingleLittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(float)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return span.ReadSingleLittleEndian();
        }

        public static ushort ReadUInt16LittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(ushort)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadUInt16LittleEndian(span);
        }

        public static uint ReadUInt32LittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(uint)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadUInt32LittleEndian(span);
        }

        public static ulong ReadUInt64LittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(ulong)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadUInt64LittleEndian(span);
        }

        public static void ReadBytes(this ref SequenceReader<byte> reader, int size, IBufferWriter<byte> writer)
        {
            var span = writer.GetSpan(size);
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            writer.Advance(size);
        }

        public static string ReadString(this ref SequenceReader<byte> reader, int size, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            Span<byte> span = stackalloc byte[size];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return encoding.GetString(span);
        }
    }

}
