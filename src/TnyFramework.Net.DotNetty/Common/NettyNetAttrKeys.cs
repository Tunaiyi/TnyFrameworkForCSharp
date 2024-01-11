// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using DotNetty.Common.Utilities;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Common;

public static class NettyNetAttrKeys
{
    public static readonly AttributeKey<INetTunnel> TUNNEL =
        AttributeKey<INetTunnel>.ValueOf("Tunnel");

    public static readonly AttributeKey<DataPackageContext> WRITE_PACKAGER = AttributeKey<DataPackageContext>.ValueOf("WRITE_PACKAGER");
    public static readonly AttributeKey<DataPackageContext> READ_PACKAGER = AttributeKey<DataPackageContext>.ValueOf("READ_PACKAGER");
}
