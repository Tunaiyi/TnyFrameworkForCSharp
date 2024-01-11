using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Nats.Hosting.Demo.Base;

public class TestRpcServiceType : RpcServiceType<TestRpcServiceType>
{
    public const string TEST_SERVER_TYPE = "test-server";
    public const string TEST_CLIENT_TYPE = "test-client";

    public const string GATEWAY_RPC_SERVICE_TYPE = "gateway-rpc-service";
    public const string GAME_RPC_SERVICE_TYPE = "game-rpc-service";
    public const string BATTLE_RPC_SERVICE_TYPE = "battle-rpc-service";

    public static readonly RpcServiceType GATEWAY_RPC_SERVICE = Of(10, GATEWAY_RPC_SERVICE_TYPE);
    public static readonly RpcServiceType GAME_RPC_SERVICE = Of(20, GAME_RPC_SERVICE_TYPE);
    public static readonly RpcServiceType BATTLE_RPC_SERVICE = Of(30, BATTLE_RPC_SERVICE_TYPE);

    public static readonly RpcServiceType TEST_SERVER = Of(100, TEST_SERVER_TYPE);
    public static readonly RpcServiceType TEST_CLIENT = Of(200, TEST_CLIENT_TYPE);
}