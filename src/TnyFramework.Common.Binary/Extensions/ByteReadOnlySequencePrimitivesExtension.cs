// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

// using System;
// using System.Buffers;
// using System.Buffers.Binary;
// using System.Text;
//
// namespace TnyFramework.Common.Binary.Extensions
// {
//
//     public static partial class ByteReadOnlySequencePrimitivesExtension
//     {
//         public static ReadOnlySequence<byte> ReadByte(this ReadOnlySequence<byte> sequence, out byte value)
//         {
//             sequence;
//             var reader =
//             if (reader.TryRead(out var value))
//             {
//                 return value;
//             }
//             throw new IndexOutOfRangeException();
//         }
//
//         public static sbyte ReadSByte(this ReadOnlySequence<byte> sequence)
//         {
//             if (reader.TryRead(out var value))
//             {
//                 return (sbyte) value;
//             }
//             throw new IndexOutOfRangeException();
//         }
//
//         public static void ReadBytes(this ReadOnlySequence<byte> sequence, Span<byte> destination)
//         {
//             if (!reader.TryCopyTo(destination))
//             {
//                 throw new IndexOutOfRangeException();
//             }
//             reader.Advance(destination.Length);
//         }
//
//         public static void ReadBytes(this ReadOnlySequence<byte> sequence, int size, IBufferWriter<byte> writer)
//         {
//             var span = writer.GetSpan(size);
//             if (!reader.TryCopyTo(span))
//             {
//                 throw new IndexOutOfRangeException();
//             }
//             reader.Advance(span.Length);
//             writer.Advance(size);
//         }
//
//         public static string ReadString(this ReadOnlySequence<byte> sequence, int size, Encoding? encoding = null)
//         {
//             encoding ??= Encoding.UTF8;
//             Span<byte> span = stackalloc byte[size];
//             if (!reader.TryCopyTo(span))
//             {
//                 throw new IndexOutOfRangeException();
//             }
//             reader.Advance(span.Length);
//             return encoding.GetString(span);
//         }
//     }
//
// }


