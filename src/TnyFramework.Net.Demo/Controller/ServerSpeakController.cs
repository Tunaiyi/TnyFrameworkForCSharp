using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Command;
using TnyFramework.Net.Demo.DTO;
using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc.Attributes;
using TnyFramework.Net.Rpc.Auth;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Demo.Controller
{
    // @RpcController
    // @AuthenticationRequired({Certificates.DEFAULT_USER_TYPE, "game-client"})
    // @BeforePlugin(SpringBootParamFilterPlugin.class)
    [RpcController]
    [AuthenticationRequired(Certificate.DEFAULT_USER_TYPE, "game-client")]
    public class ServerSpeakController : IController
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<ServerSpeakController>();

        private static readonly ThreadLocal<Random> THREAD_LOCAL = new ThreadLocal<Random>();


        [RpcRequest(CtrlerIds.SPEAK_4_SAY)]
        public SayContentDTO Say(IEndpoint<long> endpoint, [MsgParam] string message)
        {
            endpoint.Send(MessageContexts.Push(Protocols.Protocol(CtrlerIds.SPEAK_4_PUSH), "因为 [" + message + "] 推条信息给你! " + Rand()));
            return new SayContentDTO(endpoint.Id, "respond " + message);
        }


        [RpcRequest(CtrlerIds.SPEAK_4_SAY_FOR_RPC)]
        public SayContentDTO SayForBody([UserId] RpcLinkerId id, [MsgParam] string message)
        {
            return new SayContentDTO(id.Id, "respond " + message);
        }


        [RpcRequest(CtrlerIds.SPEAK_4_TEST)]
        public SayContentDTO Test(IEndpoint<long> endpoint,
            [MsgParam] sbyte byteValue,
            [MsgParam] short shortValue,
            [MsgParam] int intValue,
            [MsgParam] long longValue,
            [MsgParam] float floatValue,
            [MsgParam] double doubleValue,
            [MsgParam] bool booleanValue,
            [MsgParam] string message)
        {
            var content = "\nbyteValue:" + byteValue +
                          "\nshortValue:" + shortValue +
                          "\nintValue:" + intValue +
                          "\nlongValue:" + longValue +
                          "\nfloatValue:" + floatValue +
                          "\ndoubleValue:" + doubleValue +
                          "\nbooleanValue:" + booleanValue +
                          "\nmessage:" + message;
            endpoint.Send(MessageContexts.Push(Protocols.Protocol(CtrlerIds.SPEAK_4_PUSH), "因为 [" + message + "] 推条信息给你! " + Rand()));
            return new SayContentDTO(endpoint.Id, "test result: " + content);
        }


        [RpcRequest(CtrlerIds.SPEAK_4_DELAY_SAY)]
        public async Task<SayContentDTO> DelaySay(IEndpoint<long> endpoint, [MsgParam] string message, [MsgParam] long delay)
        {
            var timeout = DateTime.Now.ToLongTimeString() + delay;
            await Task.Delay((int)delay);
            LOGGER.LogInformation(message);
            return new SayContentDTO(endpoint.Id, "delay message : " + message);
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
}