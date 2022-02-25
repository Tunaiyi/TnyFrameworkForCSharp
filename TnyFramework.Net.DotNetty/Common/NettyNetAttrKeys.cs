using DotNetty.Common.Utilities;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.DotNetty.Common
{
    public static class NettyNetAttrKeys
    {
        public static readonly AttributeKey<INetTunnel> TUNNEL =
            AttributeKey<INetTunnel>.ValueOf("Tunnel");


        public static readonly AttributeKey<DataPackageContext> WRITE_PACKAGER = AttributeKey<DataPackageContext>.ValueOf("WRITE_PACKAGER");
        public static readonly AttributeKey<DataPackageContext> READ_PACKAGER = AttributeKey<DataPackageContext>.ValueOf("READ_PACKAGER");
    }
}
