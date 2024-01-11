using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Application;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Message;
using TnyFramework.Net.Nats.Hosting.Demo.Base;
using TnyFramework.Net.Nats.Hosting.Demo.DTO;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Session;

namespace TnyFramework.Net.Nats.Hosting.Demo.Controller;

// @RpcController
// @AuthenticationRequired({Certificates.DEFAULT_USER_TYPE, "game-client"})
// @BeforePlugin(SpringBootParamFilterPlugin.class)
[RpcController]
[AuthenticationRequired(
    ContactType.DEFAULT_USER_TYPE,
    TestRpcServiceType.TEST_SERVER_TYPE,
    TestRpcServiceType.TEST_CLIENT_TYPE
)]
public class ServerSpeakController : IController
{
    private static readonly ILogger LOGGER = LogFactory.Logger<ServerSpeakController>();

    private static readonly ThreadLocal<Random> THREAD_LOCAL = new ThreadLocal<Random>();

    public ServerSpeakController()
    {
        LOGGER.LogInformation("init ServerSpeakController");
    }

    [RpcRequest(CtrlerIds.SPEAK_4_SAY)]
    public SayContentDTO Say(ISession session, string message)
    {
        session.Send(MessageContents.Push(Protocols.Protocol(CtrlerIds.SPEAK_4_PUSH),
            "因为 [" + message + "] 推条信息给你! " + Rand()));
        return new SayContentDTO(session.Id, "respond " + message);
    }

    [RpcRequest(CtrlerIds.SPEAK_4_SAY_FOR_RPC)]
    public SayContentDTO SayForBody([IdentifyToken] RpcAccessIdentify id, string message)
    {
        return new SayContentDTO(id.Id, "respond " + message);
    }

    [RpcRequest(CtrlerIds.SPEAK_4_SAY_FOR_CONTENT)]
    public SayContentDTO SayForContent([IdentifyToken] RpcAccessIdentify id, SayContentDTO content)
    {
        return new SayContentDTO(id.Id, "respond " + content.message);
    }

    [RpcRequest(CtrlerIds.SPEAK_4_TEST)]
    public SayContentDTO Test(ISession session,
        sbyte byteValue,
        short shortValue,
        int intValue,
        long longValue,
        float floatValue,
        double doubleValue,
        bool booleanValue,
        string message)
    {
        var content = "\nbyteValue:" + byteValue +
                      "\nshortValue:" + shortValue +
                      "\nintValue:" + intValue +
                      "\nlongValue:" + longValue +
                      "\nfloatValue:" + floatValue +
                      "\ndoubleValue:" + doubleValue +
                      "\nbooleanValue:" + booleanValue +
                      "\nmessage:" + message;
        session.Send(MessageContents.Push(Protocols.Protocol(CtrlerIds.SPEAK_4_PUSH),
            "因为 [" + message + "] 推条信息给你! " + Rand()));
        return new SayContentDTO(session.Id, "test result: " + content);
    }

    [RpcRequest(CtrlerIds.SPEAK_4_DELAY_SAY)]
    public async Task<SayContentDTO> DelaySay(ISession session, string message, long delay)
    {
        var timeout = DateTime.Now.ToLongTimeString() + delay;
        await Task.Delay((int) delay);
        LOGGER.LogInformation(message);
        return new SayContentDTO(session.Id, "delay message : " + message);
    }

    private static int Rand()
    {
        if (THREAD_LOCAL.Value == null)
        {
            THREAD_LOCAL.Value = new Random();
        }
        return THREAD_LOCAL.Value.Next(3000);
    }
}