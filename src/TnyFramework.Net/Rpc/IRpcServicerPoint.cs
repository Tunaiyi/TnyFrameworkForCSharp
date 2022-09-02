using System;

namespace TnyFramework.Net.Rpc
{

    public interface IRpcServicerPoint : IRpcServicer, IComparable<IRpcServicerPoint>
    {
        /// <summary>
        /// Rpc 连接点id
        /// </summary>
        long Id { get; }
    }

}
