// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.FastInvoke;
using TnyFramework.Common.FastInvoke.FuncInvoke;
using TnyFramework.Common.Result;
using TnyFramework.Net.Command;
using TnyFramework.Net.Common;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc.Exceptions;

namespace TnyFramework.Net.Rpc.Remote
{

    public static class RpcInvokerFastInvokers
    {
        private static readonly MethodInfo RCP_RESULT_CREATE_METHOD = typeof(RpcResults).GetMethods()
            .First(info => info.Name == "Result" && info.IsGenericMethod
                                                 && info.GetGenericArguments().Length == 1 && info.GetParameters().Length == 2
                                                 && info.GetParameters()[0].ParameterType == typeof(IResultCode)
                                                 && info.GetParameters()[1].Name == "message");

        public static IFastInvoker RcpResultCreator(Type resultType)
        {
            if (!typeof(IRpcResult).IsAssignableFrom(resultType))
                throw new IllegalArgumentException($"{resultType} create RcpResultCreator exception");
            var bodyType = resultType.IsGenericType ? resultType.GenericTypeArguments[0] : typeof(object);
            var resultCodeMethod = RCP_RESULT_CREATE_METHOD.MakeGenericMethod(bodyType);
            return FastFuncFactory.Invoker(resultCodeMethod, null!, null!);
        }

        public static IFastInvoker SourceFactory(Type bodyType)
        {
            var type = typeof(RpcCompleteSource<>).MakeGenericType(bodyType);
            var constructor = type.GetConstructors()[0];
            return FastFuncFactory.Invoker(constructor);
        }
    }

    public interface IRpcCompleteSource
    {
        void SetException(Exception cause);

        void SetResult(IMessage? message);

        Task Task { get; }
    }

    public class RpcCompleteSource<TBody> : IRpcCompleteSource
    {
        private readonly TaskCompletionSource<TBody?> source = new();

        private readonly Func<IMessage, object> messageTo;

        public RpcCompleteSource(Func<IMessage, object> messageTo)
        {
            this.messageTo = messageTo;
        }

        public void SetException(Exception cause)
        {
            var exception = (cause.IsNull() ? new RpcInvokeException(NetResultCode.RPC_INVOKE_FAILED) : cause.InnerException) ?? cause;
            source.SetException(exception);
        }

        public Task Task => source.Task;

        public void SetResult(IMessage? message)
        {
            if (message == null)
            {
                source.SetResult(default);
            } else
            {
                source.SetResult((TBody) messageTo(message));
            }
        }
    }

}
