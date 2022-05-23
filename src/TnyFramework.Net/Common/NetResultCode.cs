using TnyFramework.Common.Result;

namespace TnyFramework.Net.Common
{

    public class NetResultCode : ResultCode<NetResultCode>
    {
        /// <summary>
        ///  服务端返回响应编码响应异常(不断开连接)
        /// </summary>        
        public static readonly IResultCode DECODE_FAILED = Of(191, "编码响应异常", ResultLevel.Warn);

        /// <summary>
        ///  服务端返回响应编码响应异常
        /// </summary>    
        public static readonly IResultCode ENCODE_FAILED = Of(192, "编码响应异常", ResultLevel.Warn);

        /// <summary>
        ///  服务端接受请求解码格式错误(断开连接)
        /// </summary>    
        public static readonly IResultCode DECODE_ERROR = Of(193, "解码格式错误", ResultLevel.Error);

        /// <summary>
        ///  服务端返回响应编码响应异常
        /// </summary>    
        public static readonly IResultCode ENCODE_ERROR = Of(194, "编码响应错误", ResultLevel.Error);

        /// <summary>
        ///  收到的网络包超时失效
        /// </summary>    
        public static readonly IResultCode PACKET_TIMEOUT = Of(195, "网络包超时失效", ResultLevel.Warn);

        /// <summary>
        ///  网络包校验失败
        /// </summary>    
        public static readonly IResultCode PACKET_VERIFY_FAILED = Of(196, "网络包校验失败", ResultLevel.Warn);

        /// <summary>
        ///  服务端接受请求异常
        /// </summary>    
        public static readonly IResultCode SERVER_ERROR = Of(200, "服务端异常", ResultLevel.Warn);

        /// <summary>
        ///  服务端接受请求异常
        /// </summary>    
        public static readonly IResultCode SERVER_RECEIVE_EXCEPTION = Of(201, "服务端接受请求异常", ResultLevel.Warn);

        /// <summary>
        ///  服务端执行业务异常
        /// </summary>    
        public static readonly IResultCode SERVER_EXECUTE_EXCEPTION = Of(202, "服务端执行业务异常", ResultLevel.General);

        /// <summary>
        ///  请求模块不存在
        /// </summary> 
        public static readonly IResultCode SERVER_NO_SUCH_MODULE = Of(204, "请求模块不存在", ResultLevel.Warn);

        /// <summary>
        ///  请求操作不存在
        /// </summary>    
        public static readonly IResultCode SERVER_NO_SUCH_PROTOCOL = Of(205, "请求操作不存在", ResultLevel.Warn);

        /// <summary>
        ///  分发消息异常
        /// </summary>    
        public static readonly IResultCode SERVER_DISPATCH_EXCEPTION = Of(206, "分发消息异常", ResultLevel.Warn);

        /// <summary>
        ///  非法请求参数
        /// </summary>    
        public static readonly IResultCode SERVER_ILLEGAL_PARAMETERS = Of(207, "非法请求参数", ResultLevel.Warn);

        /// <summary>
        ///  无法接收该类型消息
        /// </summary>    
        public static readonly IResultCode SERVER_NO_RECEIVE_MODE = Of(208, "无法接收该类型消息", ResultLevel.Warn);

        /// <summary>
        ///  无法发送该类型消息
        /// </summary>    
        public static readonly IResultCode SERVER_NO_SEND_MODE = Of(209, "无法发送该类型消息", ResultLevel.Warn);

        /// <summary>
        ///  验证失败
        /// </summary>    
        public static readonly IResultCode VALIDATOR_FAIL_ERROR = Of(210, "验证失败", ResultLevel.Error);

        /// <summary>
        ///  用户未登录
        /// </summary>    
        public static readonly IResultCode NO_LOGIN = Of(211, "用户未登录", ResultLevel.Warn);

        /// <summary>
        ///  没有权限调用
        /// </summary>    
        public static readonly IResultCode NO_PERMISSIONS = Of(212, "用户没有权限调用", ResultLevel.Warn);

        /// <summary>
        ///  请求消息被篡改
        /// </summary>    
        public static readonly IResultCode MESSAGE_FALSIFY = Of(213, "请求消息被篡改", ResultLevel.Warn);

        /// <summary>
        ///  请求过期
        /// </summary>    
        public static readonly IResultCode REQUEST_TIMEOUT = Of(214, "请求过期", ResultLevel.Warn);

        /// <summary>
        ///  响应过期
        /// </summary>    
        public static readonly IResultCode RESPONSE_TIMEOUT = Of(215, "响应过期", ResultLevel.Warn);

        /// <summary>
        ///  会话丢失
        /// </summary>    
        public static readonly IResultCode SESSION_LOSS_ERROR = Of(216, "会话丢失", ResultLevel.Error);

        /// <summary>
        ///  会话超时
        /// </summary>    
        public static readonly IResultCode SESSION_TIMEOUT_ERROR = Of(217, "会话超时", ResultLevel.Error);

        /// <summary>
        ///  会话丢失
        /// </summary>    
        public static readonly IResultCode SESSION_CREATE_FAILED = Of(218, "会话创建失败", ResultLevel.General);

        /// <summary>
        ///  消息已处理过
        /// </summary>    
        public static readonly IResultCode MESSAGE_HANDLED = Of(219, "消息已处理过", ResultLevel.Warn);

        /// <summary>
        ///  证书无效
        /// </summary>    
        public static readonly IResultCode INVALID_CERTIFICATE_ERROR = Of(220, "验证无效", ResultLevel.Error);

        /// <summary>
        ///  验证失败
        /// </summary>    
        public static readonly IResultCode SERVER_OFFLINE_ERROR = Of(221, "服务器未上线", ResultLevel.Error);

        /// <summary>
        ///  服务端执行业务超时
        /// </summary>    
        public static readonly IResultCode EXECUTE_TIMEOUT = Of(222, "服务端执行业超时", ResultLevel.General);

        /// <summary>
        ///  用户已登录
        /// </summary>    
        public static readonly IResultCode LOGGED_IN = Of(223, "用户已登录", ResultLevel.General);

        /// <summary>
        ///  集群网络繁忙
        /// </summary>    
        public static readonly IResultCode CLUSTER_NETWORK_BUSY = Of(224, "集群网络未接通", ResultLevel.Error);

        /// <summary>
        ///  集群网络未接通
        /// </summary>    
        public static readonly IResultCode CLUSTER_NETWORK_UNCONNECTED_ERROR = Of(225, "集群网络未接通错误", ResultLevel.Error);

        /// <summary>
        ///  集群不存在
        /// </summary>    
        public static readonly IResultCode CLUSTER_NOT_EXIST_ERROR = Of(226, "集群不存在", ResultLevel.Error);

        /// <summary>
        ///  客户端IO异常
        /// </summary>    
        public static readonly IResultCode IO_EXCEPTION = Of(300, "客户端IO异常", ResultLevel.Warn);

        /// <summary>
        ///  客户端请求中断
        /// </summary>    
        public static readonly IResultCode INTERRUPTED = Of(301, "客户端请求中断", ResultLevel.Warn);

        /// <summary>
        ///  客户端请求超时
        /// </summary>    
        public static readonly IResultCode WAIT_TIMEOUT = Of(302, "客户端请求超时", ResultLevel.Warn);

        /// <summary>
        ///  远程调用异常
        /// </summary>    
        public static readonly IResultCode REMOTE_EXCEPTION = Of(303, "远程调用异常", ResultLevel.Warn);

        /// <summary>
        ///  客户端连接中断
        /// </summary>    
        public static readonly IResultCode CONNECT_INTERRUPTED = Of(304, "客户端请求中断", ResultLevel.Warn);

        /// <summary>
        ///  客户端请求服务端无响应
        /// </summary>    
        public static readonly IResultCode REMOTE_NO_RESPONSE = Of(305, "服务端无响应", ResultLevel.Warn);

        /// <summary>
        ///  客户端请求失败
        /// </summary>    
        public static readonly IResultCode REQUEST_FAILED = Of(306, "客户端请求失败", ResultLevel.Warn);

        /// <summary>
        ///  RPC服务不可用
        /// </summary>    
        public static readonly IResultCode RPC_SERVICE_NOT_AVAILABLE = Of(400, "RPC服务不可用", ResultLevel.Warn);

        /// <summary>
        ///   RPC调用失败
        /// </summary>
        public static readonly IResultCode RPC_INVOKE_FAILED = Of(401, "RPC调用异常", ResultLevel.Warn);
    }

}
