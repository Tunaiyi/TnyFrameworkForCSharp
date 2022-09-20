// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.DotNetty.Exception;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.DotNetty.Codec
{

    public class NetPacketEncodeHandler : MessageToByteEncoder<object>
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NetPacketEncodeHandler>();

        private readonly INetPacketEncoder encoder;

        private readonly bool closeOnError;

        public NetPacketEncodeHandler(INetPacketEncoder encoder, bool closeOnError)
        {
            this.encoder = encoder;
            this.closeOnError = closeOnError;
        }

        protected override void Encode(IChannelHandlerContext context, object value, IByteBuffer output)
        {
            switch (value)
            {
                case IByteBuffer buff:
                    output.WriteBytes(buff);
                    return;
                case byte[] bytes:
                    output.WriteBytes(bytes);
                    return;
                case ArraySegment<byte> segment:
                    output.WriteBytes(segment.Array, segment.Offset, segment.Count);
                    return;
                case IMessage message:
                    try
                    {
                        encoder.EncodeObject(context, message, output);
                    } catch (System.Exception e)
                    {
                        NetPacketCodecErrorHandler.HandleOnEncodeError(LOGGER, context, e, closeOnError);
                    }
                    return;
                default:
                    throw NetCodecException.CauseEncodeFailed($"can not encode {value.GetType()}");
            }
        }
    }

}
