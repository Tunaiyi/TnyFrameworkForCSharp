// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Pipelines.Sockets.Unofficial.Buffers;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Application;
using TnyFramework.Net.Message;
using TnyFramework.Net.Nats.Codecs;
using TnyFramework.Net.Nats.Core;
using TnyFramework.Net.Nats.Message;
using TnyFramework.Net.Nats.Options;
using TnyFramework.Net.Nats.Options.Extensions;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Nats.Transports
{

    public class NatsChannel
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NatsChannel>();
        private const int INIT_STATUS = 0;
        private const int STOP_STATUS = 1;
        private const int START_STATUS = 2;

        private const string ACCESS_KEY_HEAD = "accessKey";
        private const string ACTION_HEAD = "action";

        private volatile int channelIdStep;
        private volatile int status = INIT_STATUS;
        private readonly NatsConnection connection;
        private readonly CancellationTokenSource cancellation;
        private readonly CancellationToken cancellationToken;
        private readonly TaskScheduler taskScheduler;
        private volatile string localSubject;
        private volatile INatsAccessNode localNode;
        private INatsSub<ReadOnlySequence<byte>>? channelSubscription;

        private readonly Channel<NatsCommand> commandChannel;
        private readonly IMessageCodec messageCodec;
        private readonly INetworkContext context;
        private readonly IRpcServerOptions serverOptions;

        private readonly ConcurrentDictionary<string, NatsTransport> transports = new();

        private readonly NetTransportWatcher connectingWatcher;
        private Task? readTask;
        private Task? writeTask;

        public NatsChannel(IRpcServerOptions serverOptions, NatsOpts natsOpts, TaskScheduler taskScheduler,
            IMessageCodec messageCodec, INetworkContext context, long serverId, long accessId, CancellationToken token)
        {
            this.taskScheduler = taskScheduler;
            this.messageCodec = messageCodec;
            this.serverOptions = serverOptions;
            this.context = context;
            connection = new NatsConnection(natsOpts);
            cancellation = new CancellationTokenSource();
            cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellation.Token, token).Token;
            localNode = NetAccessNode.Create(serverOptions, serverId, accessId);
            commandChannel = Channel.CreateUnbounded<NatsCommand>(new UnboundedChannelOptions {
                SingleWriter = false,
                SingleReader = true,
                AllowSynchronousContinuations = false
            });
            connectingWatcher = new NetTransportWatcher(OnConnectHandle, 5000);
            var rpcOptions = serverOptions.GetRpcOptions();
            localSubject = rpcOptions.GetChannelTopic(localNode);
        }

        public INatsAccessNode LocalNode => localNode;

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="awaitWrite"></param>
        /// <returns></returns>
        internal async ValueTask Write(NatsCommand command, bool awaitWrite = false)
        {
            INatsCommandCompletable completable = command;
            try
            {
                await commandChannel.Writer.WriteAsync(command, cancellationToken);
                if (!awaitWrite)
                {
                    await completable.BeWait();
                }
            } catch (Exception e)
            {
                completable.SetException(e);
            }
        }

        /// <summary>
        /// 开始
        /// </summary>
        public async ValueTask Start()
        {
            if (cancellation.IsCancellationRequested)
            {
                return;
            }
            if (Interlocked.CompareExchange(ref status, START_STATUS, INIT_STATUS) != INIT_STATUS)
            {
                return;
            }
            await connection.ConnectAsync();
            var rpcOptions = serverOptions.GetRpcOptions();
            channelSubscription = await connection.SubscribeCoreAsync<ReadOnlySequence<byte>>(
                localSubject,
                opts: new NatsSubOpts {
                    ChannelOpts = new NatsSubChannelOpts {
                        Capacity = rpcOptions.MessageCapacity
                    }
                },
                cancellationToken: cancellationToken);
            LOGGER.LogInformation("NatsChannel {nodeKey} subscribe {subject}", localNode.NodeKey, localSubject);
            readTask = Task.Factory.StartNew(ReadLoop, cancellation.Token, TaskCreationOptions.None, taskScheduler);
            writeTask = Task.Factory.StartNew(WriteLoop, cancellation.Token, TaskCreationOptions.None, taskScheduler);
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        public async ValueTask<ITunnel?> Connect(Uri uri, ConnectedHandle? handle = null)
        {
            if (cancellation.IsCancellationRequested)
            {
                return null;
            }
            if (status == INIT_STATUS)
            {
                await Start();
            }
            var rpcUri = new NatsUri(uri);
            var accessType = rpcUri.AccessType;
            var nodeId = rpcUri.NodeId;
            var accessId = rpcUri.AccessId;
            var channelId = rpcUri.ChannelId;
            var accessor = NatsAccessor.Create(accessType, nodeId, accessId,
                channelId ?? Interlocked.Increment(ref channelIdStep));
            var transport = CreateClientTransport(accessor);
            if (!await transport.Connect())
            {
                LOGGER.LogWarning("Node {nodeKey} NatsChannel connect {uri} failed", localNode.NodeKey, uri);
                await transport.Close();
                return null;
            }
            var rpcOptions = serverOptions.GetRpcOptions();
            connectingWatcher.Watch(transport, rpcOptions.ConnectTimeout);
            if (handle == null)
            {
                LOGGER.LogInformation("Node {nodeKey} NatsChannel connect {uri} success", localNode.NodeKey, uri);
                return transport.NetTunnel;
            }
            if (await handle(transport.NetTunnel))
            {
                LOGGER.LogInformation("Node {nodeKey} NatsChannel connect {uri} success", localNode.NodeKey, uri);
                return transport.NetTunnel;
            }
            LOGGER.LogWarning("Node {nodeKey} NatsChannel connect {uri} and handle tunnel failed", localNode.NodeKey, uri);
            await transport.Close();
            return null;
        }

        private async Task ReadLoop()
        {
            if (channelSubscription == null)
            {
                throw new NullReferenceException("subscribe is null");
            }
            var reader = channelSubscription.Msgs;
            while (!cancellation.IsCancellationRequested)
            {
                var msg = await reader.ReadAsync(cancellation.Token);
                DoRead(msg);
            }
            while (reader.TryRead(out var msg))
            {
                DoRead(msg);
            }
        }

        private void DoRead(NatsMsg<ReadOnlySequence<byte>> natsMsg)
        {
            if (natsMsg.Headers == null)
            {
                return;
            }
            var accessKey = "";
            var actionValue = "";
            if (natsMsg.Headers.TryGetValue(ACTION_HEAD, out var actionHeader) &&
                natsMsg.Headers.TryGetValue(ACCESS_KEY_HEAD, out var accessKeyHeader))
            {
                accessKey = accessKeyHeader;
                actionValue = actionHeader;
            }
            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(actionValue))
            {
                return;
            }
            var action = NatsAction.OfValue(actionValue);
            var buffer = natsMsg.Data;
            NatsTransport? transport;
            switch (action.Id)
            {
                // 连接
                case NatsAction.CONNECT_ID: {
                    if (!LoadServerTransport(accessKey, out transport))
                    {
                        NotifyConnected(transport);
                        return;
                    }
                    _ = transport.Accept();
                    return;
                }

                // 已连接
                case NatsAction.CONNECTED_ID: {
                    if (!transports.TryGetValue(accessKey, out transport))
                    {
                        NotifyClosed(accessKey);
                        return;
                    }
                    transport.Connected();
                    return;
                }

                case NatsAction.PING_ID: {
                    if (!transports.TryGetValue(accessKey, out transport))
                    {
                        return;
                    }
                    transport.NetTunnel.Receive(TickMessage.Ping());
                    return;
                }

                case NatsAction.PONE_ID: {
                    if (!transports.TryGetValue(accessKey, out transport))
                    {
                        return;
                    }
                    transport.NetTunnel.Receive(TickMessage.Pong());
                    return;
                }

                // 关闭连接
                case NatsAction.CLOSE_ID: {
                    if (!transports.TryGetValue(accessKey, out transport))
                    {
                        return;
                    }
                    transports.Remove(accessKey, out _);
                    transport.Closed();
                    break;
                }

                // 消息
                case NatsAction.MESSAGE_ID: {
                    if (!transports.TryGetValue(accessKey, out transport))
                    {
                        NotifyClosed(accessKey);
                        return;
                    }
                    var message = messageCodec.Decode(natsMsg.ReplyTo, ref buffer, transport.MessageFactory);
                    transport.Receive(message);
                    break;
                }

            }
        }

        private void NotifyClosed(string accessKey)
        {
            var rpcOptions = serverOptions.GetRpcOptions();
            var accessor = NatsAccessor.Create(accessKey);
            var topic = rpcOptions.GetChannelTopic(accessor);
            _ = Write(NatsMessageCommand.Close(topic, accessKey));
        }

        private void NotifyConnected(NatsTransport transport)
        {
            _ = Write(NatsMessageCommand.Connected(transport));
        }

        /// <summary>
        /// 连接处理!
        /// </summary>
        /// <param name="watch"></param>
        private void OnConnectHandle(TransportWatch watch)
        {
            switch (watch.Status)
            {
                case TransportWatchStatus.Cancel:
                case TransportWatchStatus.Timeout: {
                    if (transports.TryRemove(watch.Key, out var transport))
                    {
                        _ = transport.Close();
                    }
                    break;
                }
            }
        }

        internal void TransportOnClose(NatsTransport transport)
        {
            transports.Remove(transport.RemoteAccessKey, out _);
            _ = transport.Closed();
        }

        /// <summary>
        /// 加载或创建服务器传输对象
        /// </summary>
        /// <param name="accessKey">返回key</param>
        /// <param name="transport">创建或加载的传输对象</param>
        /// <returns></returns>
        private bool LoadServerTransport(string accessKey, out NatsTransport transport)
        {
            while (true)
            {
                if (transports.TryGetValue(accessKey, out var exist))
                {
                    transport = exist;
                    return false;
                }
                var accessor = NatsAccessor.Create(accessKey);
                var rpcOptions = serverOptions.GetRpcOptions();
                var remoteTopic = rpcOptions.GetChannelTopic(accessor);
                transport = new NatsTransport(this, accessor, remoteTopic, NetAccessMode.Server, context, localNode);
                if (transports.TryAdd(accessKey, transport))
                {
                    var sessionFactory = context.SessionFactory;
                    sessionFactory.Create(context, transport.NetTunnel);
                    return true;
                }
                _ = transport.DisposeAsync();
            }
        }

        /// <summary>
        /// 创建客户端链接传输对象
        /// </summary>
        /// <param name="accessor">连接节点</param>
        /// <returns></returns>
        private NatsTransport CreateClientTransport(INatsAccessor accessor)
        {
            var key = accessor.AccessKey;
            NatsTransport? transport;
            while (!transports.TryGetValue(key, out transport))
            {
                var rpcOptions = serverOptions.GetRpcOptions();
                var remoteTopic = rpcOptions.GetChannelTopic(accessor);
                transport = new NatsTransport(this, accessor, remoteTopic, NetAccessMode.Client, context, localNode);
                if (transports.TryAdd(key, transport))
                {
                    var sessionFactory = context.SessionFactory;
                    sessionFactory.Create(context, transport.NetTunnel);
                    return transport;
                }
                _ = transport.DisposeAsync();
            }
            return transport;
        }

        private async Task WriteLoop()
        {
            var reader = commandChannel.Reader;
            var maxBufferSize = serverOptions.GetRpcOptions().WriteMaxBufferSize;
            var writer = BufferWriter<byte>.Create(maxBufferSize);
            while (!cancellation.IsCancellationRequested)
            {
                var command = await reader.ReadAsync(cancellation.Token);
                DoWrite(command, writer);
            }
            while (reader.TryRead(out var command))
            {
                DoWrite(command, writer);
            }
        }

        private void DoWrite(NatsCommand command, BufferWriter<byte> writer)
        {
            INatsCommandCompletable completable = command;
            try
            {
                var action = command.Action;
                var topic = command.Topic;
                var headers = new NatsHeaders {
                    [ACCESS_KEY_HEAD] = command.AccessKey,
                    [ACTION_HEAD] = action.Value,
                };
                ValueTask task;
                if (command.Message != null)
                {
                    var message = (INetMessage) command.Message;
                    messageCodec.Encode(message, writer);
                    var owned = writer.Flush();
                    try
                    {
                        var relayHeader = message.GetHeader(NatsMessageHeaderKeys.NATS_RELAY_HEADER);
                        var relayTo = relayHeader?.Relay;
                        task = string.IsNullOrEmpty(relayTo)
                            ? connection.PublishAsync(topic, owned.Value, headers, cancellationToken: cancellationToken)
                            : connection.PublishAsync(topic, owned.Value, headers, relayTo, cancellationToken: cancellationToken);
                    } finally
                    {
                        owned.Dispose();
                    }
                } else
                {
                    task = connection.PublishAsync(topic, headers: headers, cancellationToken: cancellationToken);
                }
                if (completable.WaitWritten)
                {
                    _ = WaitWritten(task, completable);
                }
            } catch (Exception e)
            {
                completable.SetException(e);
                LOGGER.LogError(e, "publish command exception");
            } finally
            {
                command.Dispose();
            }
        }

        private async ValueTask WaitWritten(ValueTask task, INatsCommandCompletable completable)
        {
            try
            {
                await task.ConfigureAwait(false);
                completable.Complete();
            } catch (Exception e)
            {
                completable.SetException(e);
                LOGGER.LogInformation(e, "WaitWritten exception");
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public async ValueTask Shutdown()
        {
            if (cancellation.IsCancellationRequested)
            {
                return;
            }
            Interlocked.Exchange(ref status, STOP_STATUS);
            foreach (var transport in transports)
            {
                _ = transport.Value.Close();
            }
            cancellation.Cancel();
            await Task.WhenAll(
                readTask ?? Task.CompletedTask,
                writeTask ?? Task.CompletedTask);
            foreach (var transport in transports)
            {
                await transport.Value.DisposeAsync();
            }
            transports.Clear();
        }
    }

}
