using System;
using TnyFramework.Common.Result;
namespace TnyFramework.Net.Message
{
    public class TickMessageHead : AbstractNetMessageHead
    {
        public static TickMessageHead Ping()
        {
            return new TickMessageHead(MessageMode.Ping);
        }


        public static TickMessageHead Pong()
        {
            return new TickMessageHead(MessageMode.Pong);
        }


        public TickMessageHead(MessageMode mode) : base(mode)
        {
            ProtocolId = Protocols.PING_PONG_PROTOCOL_NUM;
            Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }


        public override long ToMessage => MessageConstants.EMPTY_MESSAGE_ID;

        public override int ProtocolId { get; }

        public override int Line => 0;


        public override bool IsOwn(IProtocol protocol)
        {
            return ProtocolId == protocol.ProtocolId;
        }


        public override long Id => 0;

        public override int Code => ResultConstants.SUCCESS_CODE;

        public override long Time { get; }


        public override void AllotMessageId(long id)
        {
        }
    }
}
