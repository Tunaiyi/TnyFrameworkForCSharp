// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TnyFramework.Common.Binary
{

    public class ReadOnlySequenceStream : Stream
    {
        private static readonly Task<int> TASK_OF_ZERO = Task.FromResult(0);

        private Task<int>? lastReadTask;
        private readonly ReadOnlySequence<byte> readOnlySequence;
        private SequencePosition position;

        public ReadOnlySequenceStream(ReadOnlySequence<byte> readOnlySequence)
        {
            this.readOnlySequence = readOnlySequence;
            position = readOnlySequence.Start;
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => readOnlySequence.Length;

        public override long Position {
            get => readOnlySequence.Slice(0, position).Length;
            set => position = readOnlySequence.GetPosition(value, readOnlySequence.Start);
        }

        public override void Flush()
        {
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var remaining = readOnlySequence.Slice(position);
            var toCopy = remaining.Slice(0, Math.Min(count, remaining.Length));
            position = toCopy.End;
            toCopy.CopyTo(buffer.AsSpan(offset, count));
            return (int) toCopy.Length;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var bytesRead = Read(buffer, offset, count);
            if (bytesRead == 0)
            {
                return TASK_OF_ZERO;
            }

            if (lastReadTask?.Result == bytesRead)
            {
                return lastReadTask;
            } else
            {
                return lastReadTask = Task.FromResult(bytesRead);
            }
        }

        public override int ReadByte()
        {
            var remaining = readOnlySequence.Slice(position);
            if (remaining.Length > 0)
            {
                var result = remaining.First.Span[0];
                position = readOnlySequence.GetPosition(1, position);
                return result;
            } else
            {
                return -1;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            SequencePosition relativeTo;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    relativeTo = readOnlySequence.Start;
                    break;
                case SeekOrigin.Current:
                    if (offset >= 0)
                    {
                        relativeTo = position;
                    } else
                    {
                        relativeTo = readOnlySequence.Start;
                        offset += Position;
                    }

                    break;
                case SeekOrigin.End:
                    if (offset >= 0)
                    {
                        relativeTo = readOnlySequence.End;
                    } else
                    {
                        relativeTo = readOnlySequence.Start;
                        offset += Position;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin));
            }

            position = readOnlySequence.GetPosition(offset, relativeTo);
            return Position;
        }

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override void WriteByte(byte value) => throw new NotSupportedException();

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            foreach (var segment in readOnlySequence)
            {
#if NETFRAMEWORK
                await WriteAsync(destination, segment, cancellationToken).ConfigureAwait(false);
#else
                await destination.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
#endif
            }
        }

#if NETFRAMEWORK
        private static Task WriteAsync(Stream destination, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (MemoryMarshal.TryGetArray(buffer, out var array))
            {
                return destination.WriteAsync(array.Array!, array.Offset, array.Count, cancellationToken);
            }

            var sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            buffer.Span.CopyTo(sharedBuffer);
            return FinishWriteAsync(destination.WriteAsync(sharedBuffer, 0, buffer.Length, cancellationToken), sharedBuffer);
        }

        private static async Task FinishWriteAsync(Task writeTask, byte[] localBuffer)
        {
            try
            {
                await writeTask.ConfigureAwait(false);
            } finally
            {
                ArrayPool<byte>.Shared.Return(localBuffer);
            }
        }
#endif
    }

}
