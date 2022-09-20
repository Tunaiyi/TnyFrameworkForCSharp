// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Result;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Command
{

    //TODO Protocol 设置
    internal class DefaultRpcResult<TBody> : IRpcResult<TBody>
    {
        public TBody Body { get; }

        public IMessage Message { get; }

        public IResultCode ResultCode { get; }

        public string Description { get; }

        public DefaultRpcResult(IResultCode resultCode, IMessage message)
        {
            Body = message.BodyAs<TBody>();
            Message = message;
            ResultCode = resultCode;
            Description = resultCode.Message;
        }

        public DefaultRpcResult(IResultCode resultCode, TBody body)
        {
            Body = body;
            Message = null;
            ResultCode = resultCode;
            Description = resultCode.Message;
        }

        public DefaultRpcResult(IResultCode resultCode)
        {
            ResultCode = resultCode;
            Description = resultCode.Message;
        }

        object IRpcResult.Body => Body;

        public bool IsSuccess() => ResultCode.IsSuccess();

        public bool IsFailure() => ResultCode.IsFailure();
    }

    public static class RpcResults
    {
        private static readonly IRpcResult SUCCESS = new DefaultRpcResult<object>(ResultCode.SUCCESS, null);

        /// <summary>
        /// 创建成功响应结果
        /// </summary>
        /// <returns>结果</returns>
        public static IRpcResult Success()
        {
            return SUCCESS;
        }

        /// <summary>
        /// 创建成功响应结果
        /// </summary>
        /// <param name="message">消息</param>
        /// <typeparam name="TBody"></typeparam>
        /// <returns>返回响应结果</returns>
        public static IRpcResult<TBody> Success<TBody>(IMessage message)
        {
            return new DefaultRpcResult<TBody>(ResultCode.SUCCESS, message);
        }

        /// <summary>
        /// 创建成功响应结果
        /// </summary>
        /// <param name="body"></param>
        /// <typeparam name="TBody"></typeparam>
        /// <returns>返回响应结果</returns>
        public static IRpcResult<TBody> Success<TBody>(TBody body)
        {
            return new DefaultRpcResult<TBody>(ResultCode.SUCCESS, body);
        }

        /// <summary>
        /// 请求结果
        /// </summary>
        /// <returns>结果</returns>
        public static IRpcResult Result(IResultCode code)
        {
            return new DefaultRpcResult<object>(code);
        }

        /// <summary>
        /// 请求结果
        /// </summary>
        /// <returns>结果</returns>
        public static IRpcResult<TBody> Result<TBody>(IResultCode code)
        {
            return new DefaultRpcResult<TBody>(code);
        }

        /// <summary>
        /// 请求结果
        /// </summary>
        /// <returns>结果</returns>
        public static IRpcResult<TBody> Result<TBody>(IResultCode code, IMessage message)
        {
            return new DefaultRpcResult<TBody>(code, message);
        }

        /// <summary>
        /// 请求结果
        /// </summary>
        /// <returns>结果</returns>
        public static IRpcResult<TBody> Result<TBody>(IResultCode code, TBody body)
        {
            return new DefaultRpcResult<TBody>(code, body);
        }
    }

}
