using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.DotNetty.Exception;
using TnyFramework.Net.DotNetty.Message;
using TnyFramework.Net.DotNetty.Transport;


namespace TnyFramework.Net.DotNetty.Bootstrap
{
    public class NettyServerGuide
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NettyServerGuide>();

        private const int STATUS_STOP = 0;
        private const int STATUS_START = 1;
        private const int STATUS_CLOSING = 2;
        private const int STATUS_CLOSE = 3;

        private readonly NettyMessageHandler messageHandler = new NettyMessageHandler();

        private readonly IMessageFactory messageFactory;

        private readonly IChannelMaker<IChannel> channelMaker;

        private volatile ConcurrentDictionary<string, IChannel> channels = new ConcurrentDictionary<string, IChannel>();

        private IEventLoopGroup bossGroup;

        private IEventLoopGroup workerGroup;

        private readonly ServerSettings settings;

        private int status = STATUS_STOP;

        private IOnMessage onMessage;


        public NettyServerGuide(ServerSettings settings, IMessageFactory messageFactory, IOnMessage onMessage,
            IChannelMaker<IChannel> channelMaker)
        {
            this.onMessage = onMessage;
            this.channelMaker = channelMaker;
            this.messageFactory = messageFactory;
            this.settings = settings;
        }


        /// <summary>
        /// 服务名
        /// </summary>
        public string Name => settings.Name;


        /// <summary>
        /// 打开监听
        /// </summary>
        public async Task Open()
        {
            var current = status;
            if (current != STATUS_STOP)
                await Task.FromException(new NetException($"#NettyServer 状态为 {current} 错误"));
            if (Interlocked.CompareExchange(ref status, STATUS_START, current) == current)
            {
                await Bind(settings.Host, settings.Port);
            }
        }


        /// <summary>
        /// 关闭监听
        /// </summary>
        public async Task Close()
        {
            var current = status;
            if (current != STATUS_START)
                return;
            if (Interlocked.CompareExchange(ref status, STATUS_CLOSING, STATUS_START) == current)
            {
                var closeChannels = channels;
                channels = new ConcurrentDictionary<string, IChannel>();
                var closeTasks = closeChannels.Values.Select(channel => channel.CloseAsync()).ToList();
                await Task.WhenAll(closeTasks);
                Interlocked.Exchange(ref status, STATUS_CLOSE);
            }
        }



        private async Task Bind(string host, int port)
        {
            var addressString = ToAddressString(host, port);
            if (channels.TryRemove(addressString, out var exist))
            {
                await exist.CloseAsync();
            }


            LOGGER.LogInformation("#NettyServer [ {} ] | 正在打开监听{}:{}", Name, host, port);
            if (IPAddress.Loopback != null)
            {
                IPAddress address;
                if (host.Equals("0.0.0.0"))
                {
                    address = IPAddress.Any;
                } else
                {
                    address = IPAddress.Parse(host);
                }
                var newChannel = await Bootstrap().BindAsync(new IPEndPoint(address, port));
                if (newChannel != null)
                {
                    channels.TryAdd(addressString, newChannel);
                    LOGGER.LogInformation("#NettyServer [ {} ] | {}:{} 端口已监听", Name, host, port);
                } else
                {
                    LOGGER.LogInformation("#NettyServer [ {} ] | {}:{} 端口监听失败", Name, host, port);
                }
            }
        }


        private ServerBootstrap Bootstrap()
        {
            var bootstrap = new ServerBootstrap();
            if (settings.IsLibuv)
            {
                var dispatcher = new DispatcherEventLoopGroup();
                bossGroup = dispatcher;
                workerGroup = new WorkerEventLoopGroup(dispatcher);
                bootstrap.Channel<TcpServerChannel>();
            } else
            {
                bossGroup = new MultithreadEventLoopGroup(1);
                workerGroup = new MultithreadEventLoopGroup();
                bootstrap.Channel<TcpServerSocketChannel>();
            }

            bootstrap.Group(bossGroup, workerGroup)
                .Option(ChannelOption.SoReuseaddr, true)
                .Option(ChannelOption.SoBacklog, 2048)
                .ChildOption(ChannelOption.SoKeepalive, true)
                .ChildOption(ChannelOption.TcpNodelay, true)
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel => {
                    channelMaker?.InitChannel(channel);
                    channel.Pipeline.AddLast("MessageHandler", messageHandler);
                    var tunnel = new NettyTunnel(channel, messageFactory) {
                        OnMessage = onMessage
                    };
                    tunnel.Open();
                }));
            return bootstrap;
        }


        private static string ToAddressString(string host, int port)
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentException($"host 非法参数 {host}");
            } else
            {
                return $"{host}:{port}";
            }

        }
    }
}
