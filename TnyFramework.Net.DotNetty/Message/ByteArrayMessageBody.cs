using System.Linq;
namespace TnyFramework.Net.DotNetty.Message
{
    public class ByteArrayMessageBody : BaseMessageBody<byte[]>
    {
        public ByteArrayMessageBody(byte[] body, bool relay) : base(body, relay)
        {
        }


        protected override void DoRelease(byte[] body)
        {
        }
    }
}
