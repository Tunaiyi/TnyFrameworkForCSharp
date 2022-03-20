using DotNetty.Buffers;
using DotNetty.Common.Utilities;
namespace TnyFramework.Net.Message
{
    public class ByteBufferMessageBody : BaseMessageBody<IByteBuffer>
    {
        public ByteBufferMessageBody(IByteBuffer body, bool relay) : base(body, relay)
        {
        }


        protected override void DoRelease(IByteBuffer body)
        {
            ReferenceCountUtil.Release(body);
        }
    }
}
