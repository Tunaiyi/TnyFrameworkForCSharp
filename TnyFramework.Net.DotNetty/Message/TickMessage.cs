using TnyFramework.Common.Attribute;
namespace TnyFramework.Net.DotNetty.Message
{
    public sealed class TickMessage : AbstractNetMessage
    {
        public static TickMessage Ping()
        {
            return new TickMessage(TickMessageHead.Ping());
        }


        public static TickMessage Pong()
        {
            return new TickMessage(TickMessageHead.Pong());
        }


        private TickMessage(INetMessageHead head) : base(head)
        {
        }
    }
}
