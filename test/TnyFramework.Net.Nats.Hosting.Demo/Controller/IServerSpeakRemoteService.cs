using System.Threading.Tasks;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Nats.Hosting.Demo.Base;
using TnyFramework.Net.Nats.Hosting.Demo.DTO;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Rpc.Attributes;

namespace TnyFramework.Net.Nats.Hosting.Demo.Controller;

// @RpcController
// @AuthenticationRequired({Certificates.DEFAULT_USER_TYPE, "game-client"})
// @BeforePlugin(SpringBootParamFilterPlugin.class)
// [RpcRemoteService(TestRpcServiceType.TEST_SERVER_TYPE)]
[RpcRemoteService(TestRpcServiceType.BATTLE_RPC_SERVICE_TYPE)]
public interface IServerSpeakRemoteService
{
    [RpcRequest(CtrlerIds.SPEAK_4_SAY)]
    public Task<IRpcResult<SayContentDTO>> Say(string message);

    [RpcRequest(CtrlerIds.SPEAK_4_SAY_FOR_RPC)]
    public Task<IRpcResult<SayContentDTO>> SayForBody(string message);

    [RpcRequest(CtrlerIds.SPEAK_4_SAY_FOR_CONTENT)]
    public Task<IRpcResult<SayContentDTO>> SayForContent(SayContentDTO content);

    [RpcRequest(CtrlerIds.SPEAK_4_TEST)]
    public Task<IRpcResult<SayContentDTO>> Test(
        sbyte byteValue,
        short shortValue,
        int intValue,
        long longValue,
        float floatValue,
        double doubleValue,
        bool booleanValue,
        string message);

    [RpcRequest(CtrlerIds.SPEAK_4_DELAY_SAY)]
    public Task<IRpcResult<SayContentDTO>> DelaySay(string message, long delay);
}