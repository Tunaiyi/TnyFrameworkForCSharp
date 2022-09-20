// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using DotNetty.Transport.Channels;

namespace TnyFramework.Net.DotNetty.Codec
{

    public class DatagramChannelMaker : ChannelMaker<IChannel>
    {
        private readonly INetPacketEncoder encoder;

        private readonly bool closeOnEncodeError;

        private readonly INetPacketDecoder decoder;

        private readonly bool closeOnDecodeError;

        public DatagramChannelMaker(INetPacketEncoder encoder, bool closeOnEncodeError, INetPacketDecoder decoder, bool closeOnDecodeError)
        {
            this.encoder = encoder;
            this.closeOnEncodeError = closeOnEncodeError;
            this.decoder = decoder;
            this.closeOnDecodeError = closeOnDecodeError;
        }

        protected override void MakeChannel(IChannel channel)
        {
            var channelPipeline = channel.Pipeline;
            channelPipeline.AddLast("frameDecoder", new NetPacketDecodeHandler(decoder, closeOnDecodeError));
            channelPipeline.AddLast("encoder", new NetPacketEncodeHandler(encoder, closeOnEncodeError));
        }

        protected override void PostInitChannel(IChannel channel)
        {
        }
    }

}
