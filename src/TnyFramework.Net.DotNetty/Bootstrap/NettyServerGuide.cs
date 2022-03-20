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
using TnyFramework.Net.Base;
using TnyFramework.Net.DotNetty.Exception;
using TnyFramework.Net.DotNetty.Transport;
using TnyFramework.Net.Transport;


namespace TnyFramework.Net.DotNetty.Bootstrap
{
    public interface INettyServerGuide : INetServer
    {
        /// <summary>
        /// 打开监听
        /// </summary>
        Task Open();


        /// <summary>
        /// 关闭监听
        /// </summary>
        Task Close();
    }

    public class NettyServerGuide : INettyServerGuide
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NettyServerGuide>();

        private const int STATUS_STOP = 0;
        private const int STATUS_START = 1;
        private const int STATUS_CLOSING = 2;
        private const int STATUS_CLOSE = 3;

        private readonly NettyMessageHandler messageHandler = new NettyMessageHandler();

        private readonly IIdGenerator idGenerator = new AutoIncrementIdGenerator();

        private readonly IChannelMaker channelMaker;

        private volatile ConcurrentDictionary<string, IChannel> channels = new ConcurrentDictionary<string, IChannel>();

        private ServerBootstrap bootstrap;

        private IEventLoopGroup bossGroup;

        private IEventLoopGroup workerGroup;

        private readonly IServerSetting setting;

        private readonly INettyTunnelFactory tunnelFactory;

        private readonly INetworkContext context;

        private int status = STATUS_STOP;



        public NettyServerGuide(IServerSetting setting, INettyTunnelFactory tunnelFactory,
            INetworkContext context, IChannelMaker channelMaker)
        {
            this.tunnelFactory = tunnelFactory;
            this.context = context;
            this.channelMaker = channelMaker;
            this.setting = setting;
        }


        /// <summary>
        /// 服务名
        /// </summary>
        public string Name => setting.Name;

        public IServerSetting Setting => setting;


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
                await Bind(setting.Host, setting.Port);
            }
        }


        public bool IsOpen()
        {
            return channels.Values.Any(channel => channel.Active);
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
                await Task.WhenAll(
                    bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
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
                if (bootstrap == null)
                {
                    bootstrap = Bootstrap();
                }
                var newChannel = await bootstrap.BindAsync(new IPEndPoint(address, port));
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
            if (setting.Libuv)
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
                    try
                    {
                        channelMaker?.InitChannel(channel);
                        channel.Pipeline.AddLast("MessageHandler", messageHandler);
                        var id = idGenerator.Generate();
                        var tunnel = tunnelFactory.Create(id, channel, context);
                        tunnel.Open();
                    } catch (System.Exception e)
                    {
                        LOGGER.LogError(e, $"create {channel} channel exception");
                        channel.CloseAsync();
                        throw;
                    }

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
