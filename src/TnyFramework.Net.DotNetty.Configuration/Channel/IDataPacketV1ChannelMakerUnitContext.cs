// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Configuration.Guide;

namespace TnyFramework.Net.DotNetty.Configuration.Channel
{

    public interface IDataPacketV1ChannelMakerUnitContext
    {
        IList<IChannelPipelineChain> LoadPipelineChains(INetGuideUnitContext context);

        INetPacketDecoder LoadPackDecoder(INetGuideUnitContext context);

        INetPacketEncoder LoadPackEncoder(INetGuideUnitContext context);

        ICodecVerifier LoadCodecVerifier(INetGuideUnitContext context);

        ICodecCrypto LoadCodecCrypto(INetGuideUnitContext context);

        bool LoadCloseOnEncodeError();

        bool LoadCloseOnDecodeError();

        DataPacketV1Setting LoadEncodeConfig();

        DataPacketV1Setting LoadDecodeConfig();
    }

}
