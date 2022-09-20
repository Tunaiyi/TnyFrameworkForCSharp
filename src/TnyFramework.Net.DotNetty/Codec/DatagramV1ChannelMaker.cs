// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.DotNetty.Codec
{

    public class DatagramV1ChannelMaker : DatagramChannelMaker
    {
        public DatagramV1ChannelMaker(INetPacketEncoder encoder, bool closeOnEncodeError, INetPacketDecoder decoder, bool closeOnDecodeError) :
            base(encoder, closeOnEncodeError, decoder, closeOnDecodeError)
        {
        }

        public DatagramV1ChannelMaker(DataPacketV1Setting setting, IMessageCodec messageCodec, bool closeOnEncodeError, bool closeOnDecodeError)
            : base(new NetPacketV1Encoder(setting, messageCodec), closeOnEncodeError,
                new NetPacketV1Decoder(setting, messageCodec), closeOnDecodeError)
        {
        }
    }

}
