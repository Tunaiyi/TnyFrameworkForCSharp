// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.DotNetty.Codec
{

    public class NetPacketDecodeHandler : ByteToMessageDecoder
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NetPacketDecodeHandler>();

        private readonly INetPacketDecoder decoder;

        private readonly bool closeOnError;

        private readonly NetPacketDecodeMarker marker = new NetPacketDecodeMarker();

        public NetPacketDecodeHandler(INetPacketDecoder decoder, bool closeOnError)
        {
            this.decoder = decoder;
            this.closeOnError = closeOnError;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            try
            {
                while (input.ReadableBytes > 0)
                {
                    var messageObject = decoder.DecodeObject(context, input, marker);
                    if (messageObject is IMessage message)
                    {
                        output.Add(message);
                    } else
                    {
                        break;
                    }
                }
            } catch (Exception exception)
            {
                NetPacketCodecErrorHandler.HandleOnEncodeError(LOGGER, context, exception, closeOnError);
            }
        }
    }

}
