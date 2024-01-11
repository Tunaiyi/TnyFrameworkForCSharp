using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Nats.Codecs;
using TnyFramework.Net.Nats.Hosting.Demo.Base;
using TnyFramework.Net.Nats.Hosting.Demo.Controller;
using TnyFramework.Net.Rpc.Client;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Nats.Hosting.Demo;

public class TestHostService : IHostedService
{
    private readonly ILogger logger = LogFactory.Logger<TestHostService>();

    private volatile bool stop;

    private readonly IRpcClientFactory rpcClientFactory;
    private readonly IServerSpeakRemoteService speakRemoteService;

    public TestHostService(IRpcClientFactory rpcClientFactory, IServerSpeakRemoteService speakRemoteService)
    {
        this.rpcClientFactory = rpcClientFactory;
        this.speakRemoteService = speakRemoteService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!stop)
        {
            IClient? client = null;
            try
            {
                logger.LogInformation("RpcClientFactory : {factory} ", rpcClientFactory);
                var uri = new NatsUri(TestRpcServiceType.BATTLE_RPC_SERVICE_TYPE, 1, 0, Random.Shared.NextInt64(1000000000));
                client = await rpcClientFactory.Create(uri, 1);
                if (client != null)
                {
                    await client.Open();
                }
                var result = await speakRemoteService.SayForBody("ABCDEFG");
                logger.LogInformation("{Code} result : {result} ", result.ResultCode, result.Body?.message);
                break;
            } catch (Exception e)
            {
                logger.LogError(e, "");
                stop = true;
                client?.Close();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        stop = true;
        return Task.CompletedTask;
    }
}