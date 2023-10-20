// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Common.Exceptions;

namespace TnyFramework.Common.Result
{

    public static class DoneResults
    {
        public static bool IsSuccess(int code)
        {
            return code == ResultCode.SUCCESS_CODE;
        }

        public static bool IsSuccess(IResultCode code)
        {
            return code.Value == ResultCode.SUCCESS_CODE;
        }

        public static IDoneResult<T> Failure<TM, T>(IDoneResult<TM> result)
        {
            throw new NotImplementedException();
        }

        public static IDoneResult<T> Map<TM, T>(IDoneResult<TM> done, T mapper)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 返回一个成功的结果, value 为 默认值
        /// </summary>
        /// <returns></returns>
        public static IDoneResult Success()
        {
            return new DoneResult<object>(ResultCode.SUCCESS, default!, "");
        }

        /// <summary>
        /// 返回一个成功的结果, value 为 默认值
        /// </summary>
        /// <typeparam name="TM"></typeparam>
        /// <returns></returns>
        public static IDoneResult<TM> Success<TM>()
        {
            return new DoneResult<TM>(ResultCode.SUCCESS, default!, "");
        }

        /**
         * 返回一个成功的结果, value 不能为null
         *
         * @param value
         * @return
         */
        public static IDoneResult<TM> Success<TM>(TM value)
        {
            if (value == null)
                throw new NullReferenceException();
            return new DoneResult<TM>(ResultCode.SUCCESS, value, "");
        }

        /**
         * 返回一个成功的结果, value 可为null
         *
         * @param value
         * @return
         */
        public static IDoneResult<TM> SuccessNullable<TM>(TM value)
        {
            return new DoneResult<TM>(ResultCode.SUCCESS, value, "");
        }

        /**
         * 返回一个结果, 如果value为null时返回结果码为nullCode的结果,否则返回包含value的成功结果
         *
         * @param value    成功时结果内容
         * @param nullCode 失败时结果码
         * @return 返回结果
         */
        public static IDoneResult<TM> SuccessIfNotNull<TM>(TM value, IResultCode nullCode)
        {
            return value != null ? Success(value) : Failure<TM>(nullCode);
        }

        /**
         * 返回一个结果 可成功或失败, 由code决定
         *
         * @param value 结果值
         * @param code  结果码
         * @return 返回结果
         */
        public static IDoneResult Done(IResultCode code)
        {
            return new DoneResult<object>(code, null!, null!);
        }

        /**
         * 返回一个结果 可成功或失败, 由code决定
         *
         * @param value 结果值
         * @param code  结果码
         * @return 返回结果
         */
        public static IDoneResult<TM> Done<TM>(IResultCode code, TM value)
        {
            return new DoneResult<TM>(code, value, null!);
        }

        /**
         * 返回一个结果 可成功或失败, 由code决定
         *
         * @param value   结果值
         * @param code    结果码
         * @param message 消息
         * @return 返回结果
         */
        public static IDoneResult<TM> Done<TM>(IResultCode code, TM value, string message)
        {
            return new DoneResult<TM>(code, value, message);
        }

        /**
         * 返回结果
         *
         * @return
         */
        public static IDoneResult<TM> Failure<TM>()
        {
            return new DoneResult<TM>(ResultCode.FAILURE, default!, null!);
        }

        /**
         * 返回一个以code为结果码的失败结果, 并设置 message
         *
         * @param code 结果码
         * @return 返回结果
         */
        public static IDoneResult Failure(IResultCode code, string message = "")
        {
            if (code.IsSuccess())
            {
                throw new IllegalArgumentException($"code [{code}] is success");
            }
            return new DoneResult<object>(code, default!, null!);
        }

        /**
         * 返回一个以code为结果码的失败结果, 并设置 message
         *
         * @param code 结果码
         * @return 返回结果
         */
        public static IDoneResult<TM> Failure<TM>(IResultCode code, string message = "")
        {
            if (code.IsSuccess())
            {
                throw new IllegalArgumentException($"code [{code}] is success");
            }
            return new DoneResult<TM>(code, default!, null!);
        }

        /**
         * 返回一个结果码为result的结果码的失败结果
         *
         * @param result 失败结果
         * @return 返回结果
         */
        public static IDoneResult<TM> Failure<TM>(IDoneResult result, string? message = null)
        {
            if (result.IsSuccess())
            {
                throw new IllegalArgumentException($"code [{result.Code}] is success");
            }
            return message == null ? new DoneResult<TM>(result.Code, default!, result.Message) : new DoneResult<TM>(result.Code, default!, message);

        }

        /**
         * 返回一个DoneResults, 结果码为result的结果码, 返回内容为value的.
         *
         * @param result 失败结果
         * @param value  返回值
         * @return DoneResults
         */
        public static IDoneResult<TM> Map<TM>(IDoneResult result, TM value)
        {
            return new DoneResult<TM>(result.Code, value, result.Message);
        }

        /**
         * 返回一个DoneResults, 其结果码为result的结果码, 返回内容为mapper的返回结果.
         *
         * @param result 失败结果
         * @param mapper 返回值的mapper函数
         * @return DoneResults
         */
        public static IDoneResult<T> Map<TM, T>(this IDoneResult<TM> result, Func<IResultCode, TM, T> mapper)
        {
            return new DoneResult<T>(result.Code, mapper(result.Code, result.Value), result.Message);
        }
    }

}
