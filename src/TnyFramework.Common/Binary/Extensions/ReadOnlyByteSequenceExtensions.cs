using System;
using System.Buffers;

namespace TnyFramework.Common.Binary.Extensions
{

    public static class ReadOnlyByteSequenceExtensions
    {
        private static void Release(ReadOnlySequence<byte> sequence)
        {
            var startAt = sequence.Start;
            var startObject = startAt.GetObject();
            if (startObject is byte[] bytes)
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        private static Action<ReadOnlySequence<byte>> ReleaseAction { get; } = Release;
    }

}
