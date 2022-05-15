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

        public static IDoneResult<T> Failure<M, T>(IDoneResult<M> result)
        {
            throw new NotImplementedException();
        }

        public static IDoneResult<T> Map<M, T>(IDoneResult<M> done, T mapper)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 返回一个成功的结果, value 为 默认值
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <returns></returns>
        public static IDoneResult<M> Success<M>()
        {
            return new DoneResult<M>(ResultCode.SUCCESS, default, null);
        }

        /**
         * 返回一个成功的结果, value 不能为null
         *
         * @param value
         * @return
         */
        public static IDoneResult<M> Success<M>(M value)
        {
            if (value == null)
                throw new NullReferenceException();
            return new DoneResult<M>(ResultCode.SUCCESS, value, null);
        }

        /**
         * 返回一个成功的结果, value 可为null
         *
         * @param value
         * @return
         */
        public static IDoneResult<M> SuccessNullable<M>(M value)
        {
            return new DoneResult<M>(ResultCode.SUCCESS, value, null);
        }

        /**
         * 返回一个结果, 如果value为null时返回结果码为nullCode的结果,否则返回包含value的成功结果
         *
         * @param value    成功时结果内容
         * @param nullCode 失败时结果码
         * @return 返回结果
         */
        public static IDoneResult<M> SuccessIfNotNull<M>(M value, IResultCode nullCode)
        {
            return value != null ? Success(value) : Failure<M>(nullCode);
        }

        /**
         * 返回一个结果 可成功或失败, 由code决定
         *
         * @param value 结果值
         * @param code  结果码
         * @return 返回结果
         */
        public static IDoneResult<M> Done<M>(IResultCode code, M value)
        {
            return new DoneResult<M>(code, value, null);
        }

        /**
         * 返回一个结果 可成功或失败, 由code决定
         *
         * @param value   结果值
         * @param code    结果码
         * @param message 消息
         * @return 返回结果
         */
        public static IDoneResult<M> Done<M>(IResultCode code, M value, string message)
        {
            return new DoneResult<M>(code, value, message);
        }

        /**
         * 返回结果
         *
         * @param <M>
         * @return
         */
        public static IDoneResult<M> Failure<M>()
        {
            return new DoneResult<M>(ResultCode.FAILURE, default, null);
        }

        /**
         * 返回一个以code为结果码的失败结果, 并设置 message
         *
         * @param code 结果码
         * @return 返回结果
         */
        public static IDoneResult<M> Failure<M>(IResultCode code, String message = "")
        {
            if (code.IsSuccess())
            {
                throw new IllegalArgumentException($"code [{code}] is success");
            }
            return new DoneResult<M>(code, default, null);
        }

        /**
         * 返回一个结果码为result的结果码的失败结果
         *
         * @param result 失败结果
         * @return 返回结果
         */
        public static IDoneResult<M> Failure<M>(IDoneResult result, string message = null)
        {
            if (result.IsSuccess())
            {
                throw new IllegalArgumentException($"code [{result.Code}] is success");
            }
            return message == null ? new DoneResult<M>(result.Code, default, result.Message) : new DoneResult<M>(result.Code, default, message);

        }

        /**
         * 返回一个DoneResults, 结果码为result的结果码, 返回内容为value的.
         *
         * @param result 失败结果
         * @param value  返回值
         * @return DoneResults
         */
        public static IDoneResult<M> Map<M>(IDoneResult result, M value)
        {
            return new DoneResult<M>(result.Code, value, result.Message);
        }

        /**
         * 返回一个DoneResults, 其结果码为result的结果码, 返回内容为mapper的返回结果.
         *
         * @param result 失败结果
         * @param mapper 返回值的mapper函数
         * @param <M>    返回类型
         * @return DoneResults
         */
        public static IDoneResult<T> Map<M, T>(this IDoneResult<M> result, Func<IResultCode, M, T> mapper)
        {
            return new DoneResult<T>(result.Code, mapper(result.Code, result.Value), result.Message);
        }
    }

}
