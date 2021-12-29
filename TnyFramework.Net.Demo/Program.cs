using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Result;
using TnyFramework.Net.Demo.DTO;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Message;
using TnyFramework.Net.DotNetty.Transport;
using TnyFramework.Net.TypeProtobuf;
using static TnyFramework.Net.Demo.DTO.DTOOutline;

namespace TnyFramework.Net.Demo
{
    internal class Program
    {
        public static readonly ILogger LOGGER = LogFactory.Logger<Program>();

        private class TestOnMessage : IOnMessage
        {
            public async Task OnMessage(INetTunnel tunnel, IMessage message)
            {
                if (message.ProtocolId != 100_01)
                    return;
                var paramList = message.BodyAs<IList>();
                if (paramList == null)
                    return;
                var uid = (long)paramList[1];
                var dto = new LoginDTO {
                    userId = uid,
                    certId = 2000011
                };
                dto.message = $"{dto.userId} - {dto.certId} 登录成功 at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
                LOGGER.LogInformation("will send Current thread {}", Thread.CurrentThread.ManagedThreadId);
                var send = tunnel.Send(MessageContexts.Respond(DefaultResultCode.SUCCESS, message)
                    .WithBody(dto));
                await send.Written();
                LOGGER.LogInformation("sent : Current thread {}", Thread.CurrentThread.ManagedThreadId);
            }
        }


        private static async Task Main(string[] args)
        {

            RegisterDTOs();

            var settings = new ServerSettings {
                Name = "CSTest",
                Port = 18800
            };


            var codeConfig = new DataPacketV1Config();

            var messageBody = new TypeProtobufMessageBodyCodec();

            var messageCode = new NettyMessageCodec(messageBody);

            var channelMaker = new DatagramV1ChannelMaker(codeConfig, messageCode, true, true);

            var messageFactory = new CommonMessageFactory();

            var guide = new NettyServerGuide(settings, messageFactory, new TestOnMessage(), channelMaker);

            await guide.Open();

            Console.ReadLine();

            await guide.Close();
        }
    }
}
