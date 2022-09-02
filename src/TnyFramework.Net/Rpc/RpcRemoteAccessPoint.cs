using System;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc
{

    public interface IRpcRemoteAccessPoint : ISender, IComparable<IRpcRemoteAccessPoint>
    {
        RpcAccessIdentify AccessId { get; }

        Message.ForwardPoint ForwardPoint { get; }
    }

}
