// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Result;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Codec
{

    internal static class NetPacketCodecErrorHandler
    {
        public static void HandleOnDecodeError(ILogger logger, IChannelHandlerContext ctx, System.Exception exception, bool close)
        {
            HandleOnError("Message解码", logger, ctx, exception, close);
        }

        public static void HandleOnEncodeError(ILogger logger, IChannelHandlerContext ctx, System.Exception exception, bool close)
        {
            HandleOnError("Message编码", logger, ctx, exception, close);
        }

        private static void HandleOnError(string action, ILogger logger, IChannelHandlerContext ctx, System.Exception exception, bool close)
        {
            ITunnel tunnel = null;
            IChannel channel = null;
            if (ctx != null)
            {
                channel = ctx.Channel;
                tunnel = channel.GetAttribute(NettyNetAttrKeys.TUNNEL).Get();
            }
            if (channel != null)
            {
                if (!close)
                {
                    IResultCode code = null;
                    if (exception is ResultCodeException ex)
                    {
                        code = ex.Code;
                    }
                    if (code != null && code.Level == ResultLevel.Error)
                    {
                        close = true;
                    }
                }
                if (close)
                {
                    channel.CloseAsync();
                }
            }
            logger.LogError(exception, "# Tunnel ({Tunnel}) [{Channel}] {Action}异常 {Msg}", tunnel, channel, action, close ? ", 服务器主动关闭连接" : "");
        }
    }

}
