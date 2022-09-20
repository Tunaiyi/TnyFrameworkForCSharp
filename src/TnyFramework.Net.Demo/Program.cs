using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Result;
using TnyFramework.DI.Container;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
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
                var uid = (long) paramList[1];
                var dto = new LoginDTO {
                    userId = uid,
                    certId = 2000011
                };
                dto.message = $"{dto.userId} - {dto.certId} 登录成功 at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
                LOGGER.LogInformation("will send Current thread {ThreadId}", Thread.CurrentThread.ManagedThreadId);
                var send = tunnel.Send(MessageContexts.Respond(ResultCode.SUCCESS, message)
                    .WithBody(dto));
                await send.Written();
                LOGGER.LogInformation("sent : Current thread {ThreadId}", Thread.CurrentThread.ManagedThreadId);
            }
        }

        public interface ITestInterface
        {
        }

        public interface ITestOtherInterface
        {
            string Name { get; }
        }

        public class MyTester
        {
            private readonly ITestOtherInterface testInterface;
            private readonly IServiceProvider serviceProvider;

            public MyTester(ITestOtherInterface testInterface, IServiceProvider serviceProvider)
            {
                this.testInterface = testInterface;
                this.serviceProvider = serviceProvider;
            }

            public void Say()
            {
                Console.WriteLine($"MyTester Call {testInterface.Name} : Say " + serviceProvider.GetService<MyTester>());
            }
        }

        public class MyClass : ITestInterface, ITestOtherInterface
        {
            public string Name { get; }

            public MyClass()
            {
                Console.WriteLine("New MyClass");
                Name = "MyClass";
            }
        }

        public class OtherClass : ITestInterface
        {
            public string Name { get; }

            public OtherClass()
            {
                Console.WriteLine("New OtherClass");
                Name = "OtherClass";
            }
        }

        public class OnlyTestOtherInterfaceClass : ITestOtherInterface
        {
            public OnlyTestOtherInterfaceClass()
            {
                Name = "OnlyTestOtherInterfaceClass";
            }

            public string Name { get; }
        }

        public class OnlyTestInterfaceClass : ITestInterface
        {
            public OnlyTestInterfaceClass()
            {
                Name = "OnlyTestInterfaceClass";
            }

            public string Name { get; }
        }

        public class TestModule : IApplicationModule
        {
            public void Initialize(IServiceCollection collection)
            {
            }

            public void Close(IServiceCollection collection)
            {
                throw new NotImplementedException();
            }
        }

        public class TestAppType : AppType<TestAppType>
        {
            public static readonly TestAppType TEST_CLIENT = Of(100, "TestClient");
            public static readonly TestAppType TEST_SERVICE = Of(200, "TestService");
        }

        public class TestRpcServiceType : RpcServiceType<TestRpcServiceType>
        {
            public static readonly TestRpcServiceType TEST_CLIENT = Of(100, TestAppType.TEST_CLIENT, "TestClient");
            public static readonly TestRpcServiceType TEST_SERVICE = Of(200, TestAppType.TEST_SERVICE, "TestService");
        }

        private static async Task Main(string[] args)
        {
            var rcpResultStringCreator = RpcInvokerFastInvokers.RcpResultCreator(typeof(IRpcResult<string>));
            var rpcResultString = rcpResultStringCreator.Invoke(null, ResultCode.SUCCESS, "abc");
            var rcpResultCreator = RpcInvokerFastInvokers.RcpResultCreator(typeof(IRpcResult));
            var rpcResult = rcpResultCreator.Invoke(null, ResultCode.SUCCESS, null);

            NetMessagerType.GetValues();
            var test = new ServiceCollection();
            test.AddSingletonUnit<OtherClass>("OtherClass")
                .AddSingletonUnit<MyClass>("MyClass")
                .AddSingletonUnit<MyTester>("MyTester");
            var testProvider = test.BuildServiceProvider();
            var testInterfaces = testProvider.GetService<IUnitCollection<ITestInterface>>();
            var myTester = testProvider.GetService<MyTester>();
            var myInterfaces = testProvider.GetService<IUnitCollection<MyTester>>();

            Console.WriteLine($"{testProvider.GetService<MyClass>() == testInterfaces["MyClass"]}");

            foreach (var (key, value) in testInterfaces)
            {
                Console.WriteLine($"{key} : {value.GetType()}");
            }
            foreach (var (key, value) in myInterfaces)
            {
                Console.WriteLine($"{key} : {value.GetType()}");
            }
            Func<string, object> func = value => value;
            Console.WriteLine(typeof(Func<,>).GetGenericTypeDefinition());
            Console.WriteLine(func.GetType().GetGenericTypeDefinition() == typeof(Func<,>).GetGenericTypeDefinition());

            LogFactory.Logger<Program>().LogInformation("============");
            var services = new ServiceCollection();
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ICertificateFactory<>), typeof(CertificateFactory<>)));
            var p = services.BuildServiceProvider();
            var intF = p.GetService<ICertificateFactory<int>>();
            var longF = p.GetService<ICertificateFactory<long>>();
            var fs = p.GetServices(typeof(ICertificateFactory));
            // var onMessage = p.GetService<TestOnMessage>();
            var user = new RpcAccessIdentify(TestRpcServiceType.TEST_CLIENT, 300, 1);
            var token = new RpcAccessToken(TestRpcServiceType.TEST_SERVICE, 200, user);

            var json = JsonConvert.SerializeObject(token);
            Console.WriteLine(json);
            var newToken = JsonConvert.DeserializeObject<RpcAccessToken>(json);
            Console.WriteLine(newToken);

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
            RpcServerConfiguration.CreateRpcServer(unitContainer)
                .RpcServer("game-service", 16800)
                .Server<long>("game-server", serverSpec => serverSpec.Server(18800))
                .EndpointConfigure(endpointSpec => endpointSpec
                    .SessionKeeperFactory<long>("defaultSessionKeeperFactory")
                    .CustomSessionConfigure(settingSpec => settingSpec
                        .UserType(MessagerType.DEFAULT_USER_TYPE)
                        .KeeperFactory("defaultSessionKeeperFactory")))
                .AddController<LoginController>()
                .AddController<ServerSpeakController>()
                .AuthenticateValidatorsConfigure(spec => spec.Add<DemoAuthenticateValidator>())
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
