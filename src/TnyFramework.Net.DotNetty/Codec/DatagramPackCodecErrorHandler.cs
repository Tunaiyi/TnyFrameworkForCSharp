using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Exception;
using TnyFramework.Common.Result;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.DotNetty.Codec
{
    internal static class DatagramPackCodecErrorHandler
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
            logger.LogError(exception, "# Tunnel ({}) [{}] {}异常 {}", tunnel, channel, action, close ? ", 服务器主动关闭连接" : "");
        }
    }
}
