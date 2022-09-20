// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Common.Result
{

    public interface IDone
    {
        /// <summary>
        /// 是否成功 code
        /// </summary>
        /// <returns></returns>
        bool IsSuccess();

        /// <summary>
        /// 是否成功 code
        /// </summary>
        /// <returns></returns>
        bool IsFailure();

        /// <summary>
        /// 是否有结果值
        /// </summary>
        /// <returns></returns>
        bool IsPresent();

        /// <summary>
        /// 结果消息
        /// </summary>
        string Message { get; }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        object GetValue();
    }

    public interface IDone<out TValue> : IDone
    {
        /// <summary>
        /// 返回值
        /// </summary>
        TValue Value { get; }
    }

    public static class DoneExtensions
    {
        /// <summary>
        /// 获取值,如果值为 null, 则返回 other
        /// </summary>
        /// <param name="done">完成对象</param>
        /// <param name="other">默认值</param>
        /// <typeparam name="M"></typeparam>
        /// <returns></returns>
        public static M OrElse<M>(this IDone<M> done, M other)
        {
            var value = done.Value;
            return value != null ? value : other;
        }

        /// <summary>
        /// 获取值,如果值为 null, 则返回 other的返回值
        /// </summary>
        /// <param name="done">完成对象</param>
        /// <param name="other">默认值</param>
        /// <typeparam name="M"></typeparam>
        /// <returns></returns>
        public static M OrElseGet<M>(this IDone<M> done, Func<M> other)
        {
            var value = done.Value;
            return value != null ? value : other();
        }

        /// <summary>
        /// 获取值,如果值为 null, 则抛出exceptionFunc返回的异常
        /// </summary>
        /// <param name="done">完成对象</param>
        /// <param name="exceptionFunc">异常提供器</param>
        /// <typeparam name="M"></typeparam>
        /// <returns></returns>
        public static M OrElseThrow<M>(this IDone<M> done, Exception exception)
        {
            var value = done.Value;
            if (value != null)
                return value;
            throw exception;
        }

        /// <summary>
        /// 获取值,如果值为 null, 则抛出exceptionFunc返回的异常
        /// </summary>
        /// <param name="done">完成对象</param>
        /// <param name="exceptionFunc">异常提供器</param>
        /// <typeparam name="M"></typeparam>
        /// <returns></returns>
        public static M OrElseThrow<M>(this IDone<M> done, Func<Exception> exceptionFunc)
        {
            var value = done.Value;
            if (value != null)
                return value;
            throw exceptionFunc();
        }

        /// <summary>
        /// 如果值存在则执行 Action<M> run
        /// </summary>
        /// <param name="done">完成对象</param>
        /// <param name="run">执行方法</param>
        /// <typeparam name="M"></typeparam>
        public static void IfPresent<M>(this IDone<M> done, Action<M> run)
        {
            if (done.IsPresent())
                run(done.Value);
        }

        /// <summary>
        /// 如果成功则执行 Action<M> run
        /// </summary>
        /// <param name="done">完成对象</param>
        /// <param name="run">执行方法</param>
        /// <typeparam name="M"></typeparam>
        public static void IfSuccess<M>(this IDone<M> done, Action<M> run)
        {
            if (done.IsSuccess())
                run(done.Value);
        }

        /// <summary>
        /// 是否失败
        /// </summary>
        /// <param name="done">完成对象</param>
        /// <typeparam name="M"></typeparam>
        /// <returns></returns>
        public static bool IsFailure<M>(this IDone<M> done)
        {
            return !done.IsSuccess();
        }

        /// <summary>
        /// 如果失败则执行 Action<M> run
        /// </summary>
        /// <param name="done">完成对象</param>
        /// <param name="run">执行方法</param>
        /// <typeparam name="M"></typeparam>
        public static void IfFailure<M>(this IDone<M> done, Action<M> run)
        {
            if (done.IsFailure())
                run(done.Value);
        }
    }

}
