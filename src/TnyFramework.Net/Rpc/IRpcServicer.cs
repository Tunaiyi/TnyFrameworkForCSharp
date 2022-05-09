using System;

namespace TnyFramework.Net.Rpc
{

    public interface IRpcServicer : IComparable<IRpcServicer>
    {
        IRpcServiceType ServiceType { get; }

        int ServerId { get; }

        long Id { get; }

    }

}
