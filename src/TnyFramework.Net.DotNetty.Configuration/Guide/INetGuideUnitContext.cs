using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Configuration.Channel;
using TnyFramework.Net.DotNetty.Transport;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.DotNetty.Configuration.Guide
{
    public interface INetGuideUnitContext
    {
        IChannelMaker LoadChannelMaker();

        INettyTunnelFactory LoadTunnelFactory();

        IMessageFactory LoadMessageFactory();

        IMessageCodec LoadMessageCodec();

        IMessageBodyCodec LoadMessageBodyCodec();

        INetworkContext LoadNetworkContext();

        IDataPacketV1ChannelMakerUnitContext ChannelMakerUnitContext { get; }

        INetUnitContext UnitContext { get; }
    }

    public interface INetGuideUnitContext<TUserId> : INetGuideUnitContext
    {
        ICertificateFactory<TUserId> LoadCertificateFactory();
    }
}
