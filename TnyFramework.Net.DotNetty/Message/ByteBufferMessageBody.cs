using System.Threading;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using TnyFramework.Net.DotNetty.Common;
namespace TnyFramework.Net.DotNetty.Message
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
