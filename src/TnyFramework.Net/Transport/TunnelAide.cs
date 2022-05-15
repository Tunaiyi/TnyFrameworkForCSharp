using TnyFramework.Common.Result;

namespace TnyFramework.Net.Transport
{

    public static class TunnelAide
    {
        public static ISendReceipt ResponseMessage(INetTunnel tunnel, MessageContext context)
        {
            var code = context.ResultCode;
            var receipt = tunnel.Send(context);
            if (code.Level == ResultLevel.Error)
            {
                tunnel.Close();
            }
            return receipt;
        }
    }

}
