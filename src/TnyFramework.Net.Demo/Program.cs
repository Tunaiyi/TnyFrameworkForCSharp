// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Result;
using TnyFramework.DI.Container;
using TnyFramework.Net.Base;
using TnyFramework.Net.Demo.Controller;
using TnyFramework.Net.Demo.DTO;
using TnyFramework.Net.DotNetty.Configuration;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Rpc.Remote;
using TnyFramework.Net.Transport;
using static TnyFramework.Net.Demo.DTO.DTOOutline;

namespace TnyFramework.Net.Demo
{

    internal class Program
    {
        static Program()
        {
            // ConsoleLoggerContext.Init();
        }

        private class TestOnMessage : IOnMessage
        {
            public async Task OnMessage(INetTunnel tunnel, IMessage message)
            {
                var LOGGER = LogFactory.Logger<Program>();
                if (message.ProtocolId != 100_01)
                    return;
                var paramList = message.BodyAs<IList>();
                if (paramList == null)
                    return;
                var uid = (long) paramList[1]!;
                var dto = new LoginDTO {
                    userId = uid,
                    certId = 2000011
                };
                dto.message = $"{dto.userId} - {dto.certId} 登录成功 at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
                LOGGER.LogInformation("will send Current thread {ThreadId}", Thread.CurrentThread.ManagedThreadId);
                var sent = tunnel.Send(MessageContents.Respond(ResultCode.SUCCESS, message)
                    .WithBody(dto));
                await sent;
                LOGGER.LogInformation("sent : Current thread {ThreadId}", Thread.CurrentThread.ManagedThreadId);
            }
        }

        public class TestRpcServiceType : RpcServiceType<TestRpcServiceType>
        {
            public static readonly RpcServiceType GAME = Of(100, "game-service");
            public static readonly RpcServiceType GAME_CLIENT = Of(200, "game-client");
        }

        private static async Task Main(string[] args)
        {

            TestRpcServiceType.GetValues();
            NetContactType.GetValues();
            RegisterDTOs();

            // unitContainer.BindSingleton(new ServerSetting {
            //         Name = "CSTest",
            //         Port = 18800
            //     })
            //     .BindSingleton<DataPacketV1Setting>()
            //     .BindSingleton<DatagramV1ChannelMaker>()
            //     .BindSingleton<CommonMessageFactory>()
            //     .BindSingleton<TypeProtobufMessageBodyCodec>()
            //     .BindSingleton<NettyMessageCodec>();

            var unitContainer = new ServiceCollection();
            NettyRpcHostServerConfiguration.CreateRpcServer(unitContainer)
                .RpcServer("game-service", 17800)
                .Server("game-server", serverSpec => serverSpec.Server(18800))
                .EndpointConfigure(endpointSpec => endpointSpec
                    .SessionKeeperFactory("defaultSessionKeeperFactory")
                    .CustomSessionConfigure(settingSpec => settingSpec
                        .UserType(ContactType.DEFAULT_USER_TYPE)
                        .KeeperFactory("defaultSessionKeeperFactory")))
                .AddController<LoginController>()
                .AddController<ServerSpeakController>()
                .AuthenticateValidatorsConfigure(spec => spec.Add<DemoAuthenticationValidator>())
                .Initialize();
            var provider = unitContainer.BuildServiceProvider();
            var application = provider.GetService<INetApplication>();
            if (application != null)
            {
                await application.Start();
            }
            Console.ReadLine();

            // await guide.Close();
        }
    }

}
