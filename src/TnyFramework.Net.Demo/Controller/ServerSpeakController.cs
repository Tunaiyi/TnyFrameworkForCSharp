// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Demo.DTO;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Demo.Controller
{

    // @RpcController
    // @AuthenticationRequired({Certificates.DEFAULT_USER_TYPE, "game-client"})
    // @BeforePlugin(SpringBootParamFilterPlugin.class)
    [RpcController]
    [AuthenticationRequired(ContactType.DEFAULT_USER_TYPE, "game-client")]
    public class ServerSpeakController : IController
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<ServerSpeakController>();

        private static readonly ThreadLocal<Random> THREAD_LOCAL = new ThreadLocal<Random>();

        [RpcRequest(CtrlerIds.SPEAK_4_SAY)]
        public SayContentDTO Say(IEndpoint endpoint, string message)
        {
            endpoint.Send(MessageContents.Push(Protocols.Protocol(CtrlerIds.SPEAK_4_PUSH), "因为 [" + message + "] 推条信息给你! " + Rand()));
            return new SayContentDTO(endpoint.Id, "respond " + message);
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
        public SayContentDTO Test(IEndpoint endpoint,
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
            endpoint.Send(MessageContents.Push(Protocols.Protocol(CtrlerIds.SPEAK_4_PUSH), "因为 [" + message + "] 推条信息给你! " + Rand()));
            return new SayContentDTO(endpoint.Id, "test result: " + content);
        }

        [RpcRequest(CtrlerIds.SPEAK_4_DELAY_SAY)]
        public async Task<SayContentDTO> DelaySay(IEndpoint endpoint, string message, long delay)
        {
            var timeout = DateTime.Now.ToLongTimeString() + delay;
            await Task.Delay((int) delay);
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
