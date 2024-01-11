// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Buffers;
using System.Threading;
using TnyFramework.Common.Pools;

namespace TnyFramework.Common.Binary;

public interface IByteBufferWriter : IBufferWriter<byte>, IDisposable
{
    public int Length { get; }
}

public class ByteBufferWriter : IByteBufferWriter
{
    private static readonly AnyPool<ByteBufferWriter> POOL =
        new DefaultPool<ByteBufferWriter>(new DefaultPooledPolicy<ByteBufferWriter>());

    private IBufferWriter<byte> writer = null!;
    private volatile int length = 0;

    internal static IByteBufferWriter Wrap(IBufferWriter<byte> writer)
    {
        var ins = POOL.Get();
        ins.Init(writer);
        return ins;
    }

    private void Init(IBufferWriter<byte> writer)
    {
        this.writer = writer;
        length = 0;
    }

    public int Length => length;

    public void Advance(int count)
    {
        writer.Advance(count);
        Interlocked.Add(ref length, count);
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        return writer.GetMemory(sizeHint);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        return writer.GetSpan(sizeHint);
    }

    public void Dispose()
    {
        writer = null!;
        length = 0;
        POOL.Return(this);
    }
}
